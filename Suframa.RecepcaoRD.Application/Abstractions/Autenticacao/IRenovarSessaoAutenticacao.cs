using Suframa.RecepcaoRD.Application.Features.Autenticacao;

namespace Suframa.RecepcaoRD.Application.Abstractions.Autenticacao;

public interface IRenovarSessaoAutenticacao
{
  Task<ResultadoRenovarSessaoAutenticacao> ExecutarAsync(
    string? refreshToken,
    CancellationToken cancellationToken = default);
}
