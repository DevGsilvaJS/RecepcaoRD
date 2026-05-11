using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Suframa.RecepcaoRD.API.Extensions;

public static class CorsServiceCollectionExtensions
{
  public const string DefaultCorsPolicyName = "DefaultCorsPolicy";

  public static IServiceCollection AddCorsFromConfiguration(this IServiceCollection services, IConfiguration configuration)
  {
    string[] allowedOrigins = configuration
      .GetSection("Cors:AllowedOrigins")
      .Get<string[]>() ?? [];

    services.AddCors(options =>
    {
      options.AddPolicy(DefaultCorsPolicyName, policy =>
      {
        if (allowedOrigins.Length == 0)
        {
          policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
          return;
        }

        policy
          .WithOrigins(allowedOrigins)
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
      });
    });

    return services;
  }
}

