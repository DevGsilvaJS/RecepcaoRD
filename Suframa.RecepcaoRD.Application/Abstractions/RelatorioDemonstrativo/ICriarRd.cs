using Suframa.RecepcaoRD.Application.DTOs.RelatorioDemonstrativo;

namespace Suframa.RecepcaoRD.Application.Abstractions.RelatorioDemonstrativo;

public interface ICriarRd
{
  Task CriarAsync(CriarRdRecepcaoDto requisicao, CancellationToken cancellationToken = default);
}
