using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Suframa.RecepcaoRD.Application.Abstractions.Cadsuf;
using Suframa.RecepcaoRD.Application.Abstractions.Persistence;
using Suframa.RecepcaoRD.Application.Abstractions.Security;
using Suframa.RecepcaoRD.Infrastructure.Persistence;
using Suframa.RecepcaoRD.Infrastructure.Persistence.Repositories;
using Suframa.RecepcaoRD.Infrastructure.Security.Jwt;

namespace Suframa.RecepcaoRD.API.Extensions;

public static class PersistenceServiceCollectionExtensions
{
  public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
  {
    string? cadsufDb = configuration.GetConnectionString("CadsufDB");
    if (string.IsNullOrWhiteSpace(cadsufDb))
    {
      throw new InvalidOperationException(
        "ConnectionStrings:CadsufDB não está definida ou está vazia. " +
        "Defina em appsettings.json, appsettings.{Ambiente}.json ou variáveis de ambiente. " +
        "Em desenvolvimento local, use ASPNETCORE_ENVIRONMENT=Development para carregar appsettings.Development.json.");
    }

    services.AddDbContext<CadsufDbContext>(options =>
    {
      options.UseSqlServer(cadsufDb);
    });

    services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<RecepcaoRDDbContext>());
    services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    services.AddScoped<ICadsufCompanyRepository, CadsufCompanyRepository>();

    services.Configure<JwtOptions>(o =>
    {
      o.TokenSecret = configuration["TokenSecret"] ?? string.Empty;
    });
    services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
    return services;
  }
}

