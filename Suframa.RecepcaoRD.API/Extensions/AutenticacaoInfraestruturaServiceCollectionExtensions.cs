using Microsoft.Extensions.DependencyInjection;
using Suframa.RecepcaoRD.Application.Abstractions.Autenticacao;
using Suframa.RecepcaoRD.Application.Abstractions.Security;
using Suframa.RecepcaoRD.Infrastructure.Autenticacao;
using Suframa.RecepcaoRD.Infrastructure.Security.Jwt;

namespace Suframa.RecepcaoRD.API.Extensions;

public static class AutenticacaoInfraestruturaServiceCollectionExtensions
{
  public static IServiceCollection AddAutenticacaoInfraestrutura(this IServiceCollection services)
  {
    services.AddSingleton<IArmazenamentoRefreshToken, ArmazenamentoRefreshTokenEmMemoria>();
    services.AddSingleton<IJwtLeitorExpiracao, JwtLeitorExpiracao>();
    services.AddSingleton<IJwtLeitorReivindicacoes, JwtLeitorReivindicacoes>();
    return services;
  }
}
