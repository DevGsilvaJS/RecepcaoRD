using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Suframa.RecepcaoRD.Application.Abstractions.Persistence;

namespace Suframa.RecepcaoRD.Infrastructure.Persistence.Repositories;

internal static class OrdenacaoRepositorioGenerico
{
  internal static string ResolverOrdenacaoPadrao<TEntidade>() where TEntidade : class
  {
    Type tipo = typeof(TEntidade);
    PropertyInfo? comChave = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance)
      .FirstOrDefault(p => Attribute.IsDefined(p, typeof(KeyAttribute)) && PropriedadeOrdenavel(p));
    if (comChave is not null && PropriedadeOrdenavel(comChave))
    {
      return comChave.Name;
    }

    PropertyInfo? id = tipo.GetProperty(
      "Id",
      BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    if (id is not null && PropriedadeOrdenavel(id))
    {
      return id.Name;
    }

    PropertyInfo? primeiraOrdenavel = tipo.GetProperties(BindingFlags.Public | BindingFlags.Instance)
      .FirstOrDefault(p => PropriedadeOrdenavel(p));
    if (primeiraOrdenavel is not null)
    {
      return primeiraOrdenavel.Name;
    }

    throw new InvalidOperationException(
      $"Não foi possível definir ordenação padrão para {tipo.Name}: marque a chave com [Key] ou exponha uma propriedade Id ordenável.");
  }

  internal static IOrderedQueryable<TEntidade> Aplicar<TEntidade>(
    IQueryable<TEntidade> consulta,
    string? nomeCampo,
    bool decrescente) where TEntidade : class
  {
    PropertyInfo propriedade = ResolverPropriedade<TEntidade>(nomeCampo);
    return decrescente
      ? OrdenarDecrescente(consulta, propriedade)
      : OrdenarCrescente(consulta, propriedade);
  }

  internal static IOrderedQueryable<TEntidade> AplicarThenBy<TEntidade>(
    IOrderedQueryable<TEntidade> consultaOrdenada,
    string? nomeCampo,
    bool decrescente) where TEntidade : class
  {
    PropertyInfo propriedade = ResolverPropriedade<TEntidade>(nomeCampo);
    return decrescente
      ? EntaoOrdenarDecrescente(consultaOrdenada, propriedade)
      : EntaoOrdenarCrescente(consultaOrdenada, propriedade);
  }

  private static PropertyInfo ResolverPropriedade<TEntidade>(string? nomeCampo) where TEntidade : class
  {
    Type tipo = typeof(TEntidade);
    if (!string.IsNullOrWhiteSpace(nomeCampo))
    {
      PropertyInfo? encontrada = tipo.GetProperty(
        nomeCampo.Trim(),
        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
      if (encontrada is not null && PropriedadeOrdenavel(encontrada))
      {
        return encontrada;
      }
    }

    PropertyInfo? id = tipo.GetProperty(
      "Id",
      BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    if (id is not null && PropriedadeOrdenavel(id))
    {
      return id;
    }

    throw new InvalidOperationException(
      $"Não foi possível ordenar a entidade {tipo.Name}: informe um campo válido ou defina uma propriedade Id ordenável.");
  }

  private static bool PropriedadeOrdenavel(PropertyInfo propriedade)
  {
    Type t = propriedade.PropertyType;
    Type? subjacente = Nullable.GetUnderlyingType(t);
    if (subjacente is not null)
    {
      t = subjacente;
    }

    if (t == typeof(byte[]))
    {
      return false;
    }

    if (t.IsEnum)
    {
      return true;
    }

    if (t == typeof(string))
    {
      return true;
    }

    if (t == typeof(DateTime) || t == typeof(DateTimeOffset))
    {
      return true;
    }

    if (t == typeof(decimal) || t == typeof(Guid))
    {
      return true;
    }

    return t.IsPrimitive;
  }

  private static IOrderedQueryable<TEntidade> OrdenarCrescente<TEntidade>(
    IQueryable<TEntidade> consulta,
    PropertyInfo propriedade) where TEntidade : class
  {
    ParameterExpression parametro = Expression.Parameter(typeof(TEntidade), "x");
    MemberExpression acesso = Expression.Property(parametro, propriedade);
    Type tipoChave = propriedade.PropertyType;
    Type tipoDelegado = typeof(Func<,>).MakeGenericType(typeof(TEntidade), tipoChave);
    LambdaExpression lambda = Expression.Lambda(tipoDelegado, acesso, parametro);

    MethodInfo metodoGenerico = typeof(Queryable)
      .GetRuntimeMethods()
      .Where(m => m.Name == nameof(Queryable.OrderBy))
      .Select(m => new { M = m, P = m.GetParameters() })
      .Where(x => x.M.IsGenericMethodDefinition && x.P.Length == 2)
      .Select(x => x.M)
      .First();

    MethodInfo metodo = metodoGenerico.MakeGenericMethod(typeof(TEntidade), tipoChave);
    object? resultado = metodo.Invoke(null, new object[] { consulta, lambda });
    return (IOrderedQueryable<TEntidade>)resultado!;
  }

  private static IOrderedQueryable<TEntidade> OrdenarDecrescente<TEntidade>(
    IQueryable<TEntidade> consulta,
    PropertyInfo propriedade) where TEntidade : class
  {
    ParameterExpression parametro = Expression.Parameter(typeof(TEntidade), "x");
    MemberExpression acesso = Expression.Property(parametro, propriedade);
    Type tipoChave = propriedade.PropertyType;
    Type tipoDelegado = typeof(Func<,>).MakeGenericType(typeof(TEntidade), tipoChave);
    LambdaExpression lambda = Expression.Lambda(tipoDelegado, acesso, parametro);

    MethodInfo metodoGenerico = typeof(Queryable)
      .GetRuntimeMethods()
      .Where(m => m.Name == nameof(Queryable.OrderByDescending))
      .Select(m => new { M = m, P = m.GetParameters() })
      .Where(x => x.M.IsGenericMethodDefinition && x.P.Length == 2)
      .Select(x => x.M)
      .First();

    MethodInfo metodo = metodoGenerico.MakeGenericMethod(typeof(TEntidade), tipoChave);
    object? resultado = metodo.Invoke(null, new object[] { consulta, lambda });
    return (IOrderedQueryable<TEntidade>)resultado!;
  }

  private static IOrderedQueryable<TEntidade> EntaoOrdenarCrescente<TEntidade>(
    IOrderedQueryable<TEntidade> ordenada,
    PropertyInfo propriedade) where TEntidade : class
  {
    ParameterExpression parametro = Expression.Parameter(typeof(TEntidade), "x");
    MemberExpression acesso = Expression.Property(parametro, propriedade);
    Type tipoChave = propriedade.PropertyType;
    Type tipoDelegado = typeof(Func<,>).MakeGenericType(typeof(TEntidade), tipoChave);
    LambdaExpression lambda = Expression.Lambda(tipoDelegado, acesso, parametro);

    MethodInfo metodoGenerico = typeof(Queryable)
      .GetRuntimeMethods()
      .Where(m => m.Name == nameof(Queryable.ThenBy))
      .Select(m => new { M = m, P = m.GetParameters() })
      .Where(x => x.M.IsGenericMethodDefinition && x.P.Length == 2)
      .Select(x => x.M)
      .First();

    MethodInfo metodo = metodoGenerico.MakeGenericMethod(typeof(TEntidade), tipoChave);
    object? resultado = metodo.Invoke(null, new object[] { ordenada, lambda });
    return (IOrderedQueryable<TEntidade>)resultado!;
  }

  private static IOrderedQueryable<TEntidade> EntaoOrdenarDecrescente<TEntidade>(
    IOrderedQueryable<TEntidade> ordenada,
    PropertyInfo propriedade) where TEntidade : class
  {
    ParameterExpression parametro = Expression.Parameter(typeof(TEntidade), "x");
    MemberExpression acesso = Expression.Property(parametro, propriedade);
    Type tipoChave = propriedade.PropertyType;
    Type tipoDelegado = typeof(Func<,>).MakeGenericType(typeof(TEntidade), tipoChave);
    LambdaExpression lambda = Expression.Lambda(tipoDelegado, acesso, parametro);

    MethodInfo metodoGenerico = typeof(Queryable)
      .GetRuntimeMethods()
      .Where(m => m.Name == nameof(Queryable.ThenByDescending))
      .Select(m => new { M = m, P = m.GetParameters() })
      .Where(x => x.M.IsGenericMethodDefinition && x.P.Length == 2)
      .Select(x => x.M)
      .First();

    MethodInfo metodo = metodoGenerico.MakeGenericMethod(typeof(TEntidade), tipoChave);
    object? resultado = metodo.Invoke(null, new object[] { ordenada, lambda });
    return (IOrderedQueryable<TEntidade>)resultado!;
  }
}

internal static class OrdenacaoFiltroPaginado
{
  internal static IQueryable<TEntidade> Aplicar<TEntidade>(IQueryable<TEntidade> consulta, PagedOptions filtro)
    where TEntidade : class
  {
    string? sortRaiz = string.IsNullOrWhiteSpace(filtro.Sort) ? null : filtro.Sort.Trim();
    List<SortOptions>? multiplas = filtro.SortManny?.Where(s => s is not null).ToList();
    bool semOrdenacaoInformada = string.IsNullOrEmpty(sortRaiz) && (multiplas is null || multiplas.Count == 0);
    if (semOrdenacaoInformada)
    {
      sortRaiz = OrdenacaoRepositorioGenerico.ResolverOrdenacaoPadrao<TEntidade>();
    }

    if (!string.IsNullOrEmpty(sortRaiz))
    {
      return OrdenacaoRepositorioGenerico.Aplicar(consulta, sortRaiz, filtro.Reverse);
    }

    if (multiplas is { Count: > 0 })
    {
      IOrderedQueryable<TEntidade> ordenada = OrdenacaoRepositorioGenerico.Aplicar(
        consulta,
        multiplas[0].Sort,
        multiplas[0].Reverse);
      for (int i = 1; i < multiplas.Count; i++)
      {
        ordenada = OrdenacaoRepositorioGenerico.AplicarThenBy(
          ordenada,
          multiplas[i].Sort,
          multiplas[i].Reverse);
      }

      return ordenada;
    }

    return OrdenacaoRepositorioGenerico.Aplicar(consulta, sortRaiz, filtro.Reverse);
  }
}
