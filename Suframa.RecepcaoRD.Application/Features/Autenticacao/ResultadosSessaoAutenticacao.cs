using Suframa.RecepcaoRD.Application.DTOs.Autenticacao;

namespace Suframa.RecepcaoRD.Application.Features.Autenticacao;

public abstract record ResultadoIniciarSessaoAutenticacao;

public sealed record IniciarSessaoAutenticacaoSucesso(IniciarSessaoAutenticacaoSaida Saida)
  : ResultadoIniciarSessaoAutenticacao;

public sealed record IniciarSessaoAutenticacaoEmpresaInexistente(string Mensagem)
  : ResultadoIniciarSessaoAutenticacao;

public sealed record IniciarSessaoAutenticacaoFalhaGeracaoToken(string Mensagem)
  : ResultadoIniciarSessaoAutenticacao;

public sealed record IniciarSessaoAutenticacaoSaida(
  string TokenJwt,
  DateTimeOffset ExpiraEmJwtUtc,
  string RefreshToken,
  DateTimeOffset ExpiraRefreshUtc,
  string Mensagem,
  EmpresaSessaoRespostaDto? Empresa);

public abstract record ResultadoRenovarSessaoAutenticacao;

public sealed record RenovarSessaoAutenticacaoSucesso(RenovarSessaoAutenticacaoSaida Saida)
  : ResultadoRenovarSessaoAutenticacao;

public sealed record RenovarSessaoAutenticacaoFalhou : ResultadoRenovarSessaoAutenticacao;

public sealed record RenovarSessaoAutenticacaoSaida(
  string TokenJwt,
  DateTimeOffset ExpiraEmJwtUtc,
  string RefreshToken,
  DateTimeOffset ExpiraRefreshUtc,
  string Mensagem);

public sealed record SituacaoSessaoAutenticacaoSaida(bool Autenticado, DateTimeOffset? ExpiraEmJwtUtc);
