using Suframa.RecepcaoRD.Application.DTOs.PlanosRecepcao;

namespace Suframa.RecepcaoRD.Application.Abstractions.PlanosRecepcao;

public interface IListarPlanosRecepcao
{
  Task<IReadOnlyList<PlanoRecepcaoListaItemDto>> ExecutarAsync(
    string? cnpjEmpresa,
    CancellationToken cancellationToken = default);
}
