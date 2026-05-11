using Suframa.RecepcaoRD.Application.DTOs.RelatorioDemonstrativo;

namespace Suframa.RecepcaoRD.Application.Abstractions.RelatorioDemonstrativo;

public interface IAtualizarRd
{
  Task AtualizarAsync(
    long idRd,
    AtualizarRdRecepcaoDto requisicao,
    CancellationToken cancellationToken = default);
}
