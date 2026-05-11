using System.Reflection;
using Xunit;

namespace Suframa.RecepcaoRD.Tests.Architecture;

public sealed class DomainEntitiesShouldNotUseDataAnnotationsTests
{
  private static readonly string[] ForbiddenAttributeNamespaces =
  [
    "System.ComponentModel.DataAnnotations",
    "System.ComponentModel.DataAnnotations.Schema",
  ];

  [Fact]
  public void Domain_entities_should_not_use_data_annotations()
  {
    Assembly domainAssembly = typeof(Suframa.RecepcaoRD.Domain.Entities.RdRecepcao).Assembly;

    Type[] entityTypes = domainAssembly
      .GetTypes()
      .Where(t =>
        t.IsClass &&
        !t.IsAbstract &&
        t.Namespace == "Suframa.RecepcaoRD.Domain.Entities")
      .ToArray();

    List<string> violations = [];

    foreach (Type entityType in entityTypes)
    {
      AddViolations(violations, entityType, entityType);

      foreach (PropertyInfo property in entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
      {
        AddViolations(violations, entityType, property);
      }
    }

    Assert.True(
      violations.Count == 0,
      "Domain entities must not depend on DataAnnotations.\n" + string.Join('\n', violations));
  }

  private static void AddViolations(List<string> violations, Type entityType, MemberInfo member)
  {
    foreach (CustomAttributeData attribute in member.CustomAttributes)
    {
      string? attributeNamespace = attribute.AttributeType.Namespace;
      if (attributeNamespace is null)
      {
        continue;
      }

      if (ForbiddenAttributeNamespaces.Any(ns => attributeNamespace.StartsWith(ns, StringComparison.Ordinal)))
      {
        violations.Add($"{entityType.FullName}.{member.Name} -> {attribute.AttributeType.FullName}");
      }
    }
  }
}

