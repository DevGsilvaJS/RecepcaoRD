using Microsoft.Extensions.DependencyInjection;
using Suframa.RecepcaoRD.Application.Abstractions.AuditoriaRdRecepcao;
using Suframa.RecepcaoRD.Application.Abstractions.Autenticacao;
using Suframa.RecepcaoRD.Application.Abstractions.PlanosRecepcao;
using Suframa.RecepcaoRD.Application.Abstractions.RelatorioDemonstrativo;
using Suframa.RecepcaoRD.Application.Features.Authentication;
using Suframa.RecepcaoRD.Application.Features.RegistroAuditoriaRdRecepcao;
using Suframa.RecepcaoRD.Application.Features.Autenticacao;
using Suframa.RecepcaoRD.Application.Features.PlanosRecepcao;
using Suframa.RecepcaoRD.Application.Features.RelatorioDemonstrativo;

namespace Suframa.RecepcaoRD.API.Extensions;

public static class ApplicationServiceCollectionExtensions
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    services.AddScoped<IAuthenticationTokenService, AuthenticationTokenService>();
    services.AddScoped<IIniciarSessaoAutenticacao, IniciarSessaoAutenticacao>();
    services.AddScoped<IRenovarSessaoAutenticacao, RenovarSessaoAutenticacao>();
    services.AddScoped<IEncerrarSessaoAutenticacao, EncerrarSessaoAutenticacao>();
    services.AddScoped<IObterSituacaoSessaoAutenticacao, ObterSituacaoSessaoAutenticacao>();
    services.AddScoped<IListarPlanosRecepcao, ListarPlanosRecepcao>();
    services.AddScoped<IListarRd, ListarRd>();
    services.AddScoped<ICriarRd, CriarRd>();
    services.AddScoped<IAtualizarRd, AtualizarRd>();
    services.AddScoped<IRegistrarAuditoriaRdRecepcao, RegistrarAuditoriaRdRecepcao>();
    return services;
  }
}
