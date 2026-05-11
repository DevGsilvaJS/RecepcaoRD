using Suframa.RecepcaoRD.Application.Abstractions.Autenticacao;
using Suframa.RecepcaoRD.Application.Abstractions.Security;

namespace Suframa.RecepcaoRD.Application.Features.Autenticacao;

public sealed class ObterSituacaoSessaoAutenticacao : IObterSituacaoSessaoAutenticacao
{
  private readonly IJwtLeitorExpiracao _jwtLeitor;

  public ObterSituacaoSessaoAutenticacao(IJwtLeitorExpiracao jwtLeitor)
  {
    _jwtLeitor = jwtLeitor;
  }

  public SituacaoSessaoAutenticacaoSaida Executar(string? tokenJwt)
  {
    if (string.IsNullOrWhiteSpace(tokenJwt))
    {
      return new SituacaoSessaoAutenticacaoSaida(false, null);
    }

    DateTimeOffset? expira = _jwtLeitor.ObterExpiracaoJwtUtc(tokenJwt);
    if (expira is null || expira <= DateTimeOffset.UtcNow)
    {
      return new SituacaoSessaoAutenticacaoSaida(false, null);
    }

    return new SituacaoSessaoAutenticacaoSaida(true, expira);
  }
}
