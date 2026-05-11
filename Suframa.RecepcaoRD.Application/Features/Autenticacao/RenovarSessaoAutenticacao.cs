using Suframa.RecepcaoRD.Application.Abstractions.Autenticacao;
using Suframa.RecepcaoRD.Application.Abstractions.Security;
using Suframa.RecepcaoRD.Application.Features.Authentication;

namespace Suframa.RecepcaoRD.Application.Features.Autenticacao;

public sealed class RenovarSessaoAutenticacao : IRenovarSessaoAutenticacao
{
  private readonly IAuthenticationTokenService _tokens;
  private readonly IArmazenamentoRefreshToken _refreshTokens;
  private readonly IJwtLeitorExpiracao _jwtLeitor;

  public RenovarSessaoAutenticacao(
    IAuthenticationTokenService tokens,
    IArmazenamentoRefreshToken refreshTokens,
    IJwtLeitorExpiracao jwtLeitor)
  {
    _tokens = tokens;
    _refreshTokens = refreshTokens;
    _jwtLeitor = jwtLeitor;
  }

  public async Task<ResultadoRenovarSessaoAutenticacao> ExecutarAsync(
    string? refreshToken,
    CancellationToken cancellationToken = default)
  {
    if (string.IsNullOrWhiteSpace(refreshToken))
    {
      return new RenovarSessaoAutenticacaoFalhou();
    }

    if (!_refreshTokens.TentarConsumirParaRenovacao(refreshToken, out string? documento))
    {
      return new RenovarSessaoAutenticacaoFalhou();
    }

    GenerateTokenOutcome geracao = await _tokens.GenerateTokenAsync(documento, cancellationToken);
    if (geracao is not GenerateTokenOk ok)
    {
      return new RenovarSessaoAutenticacaoFalhou();
    }

    DateTimeOffset? expiraJwt = _jwtLeitor.ObterExpiracaoJwtUtc(ok.Token);
    if (expiraJwt is null)
    {
      return new RenovarSessaoAutenticacaoFalhou();
    }

    RefreshTokenEmitido novoRefresh = _refreshTokens.EmitirParaDocumento(documento);
    var saida = new RenovarSessaoAutenticacaoSaida(
      ok.Token,
      expiraJwt.Value,
      novoRefresh.Token,
      novoRefresh.ExpiraEmUtc,
      "Sessão renovada.");

    return new RenovarSessaoAutenticacaoSucesso(saida);
  }
}
