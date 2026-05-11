using Suframa.RecepcaoRD.Application.Abstractions.Autenticacao;

namespace Suframa.RecepcaoRD.Application.Features.Autenticacao;

public sealed class EncerrarSessaoAutenticacao : IEncerrarSessaoAutenticacao
{
  private readonly IArmazenamentoRefreshToken _refreshTokens;

  public EncerrarSessaoAutenticacao(IArmazenamentoRefreshToken refreshTokens)
  {
    _refreshTokens = refreshTokens;
  }

  public void Executar(string? refreshToken)
  {
    if (!string.IsNullOrWhiteSpace(refreshToken))
    {
      _refreshTokens.Revogar(refreshToken);
    }
  }
}
