using Suframa.RecepcaoRD.Application.Abstractions.Persistence;
using Suframa.RecepcaoRD.Application.DTOs.RelatorioDemonstrativo;

namespace Suframa.RecepcaoRD.Application.Abstractions.RelatorioDemonstrativo;

public interface IListarRd
{
  Task<PagedItems<RdRecepcaoListaItemDto>> ListarAsync(
    ListarRdConsulta consulta,
    CancellationToken cancellationToken = default);
}
