using System.Diagnostics.CodeAnalysis;

namespace Suframa.RecepcaoRD.Application.Abstractions.Autenticacao;

public sealed record RefreshTokenEmitido(string Token, DateTimeOffset ExpiraEmUtc);

public interface IArmazenamentoRefreshToken
{
  RefreshTokenEmitido EmitirParaDocumento(string documentoNormalizado);

  bool TentarConsumirParaRenovacao(
    string refreshToken,
    [NotNullWhen(true)] out string? documentoNormalizado);

  void Revogar(string refreshToken);
}
