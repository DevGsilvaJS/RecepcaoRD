using Suframa.RecepcaoRD.Application.Abstractions.Persistence;
using Suframa.RecepcaoRD.Application.Abstractions.PlanosRecepcao;
using Suframa.RecepcaoRD.Application.DTOs.PlanosRecepcao;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Application.Features.PlanosRecepcao;

public sealed class ListarPlanosRecepcao : IListarPlanosRecepcao
{
  private readonly IRepository<PlanoRecepcao> _planos;

  public ListarPlanosRecepcao(IRepository<PlanoRecepcao> planos)
  {
    _planos = planos;
  }

  public async Task<IReadOnlyList<PlanoRecepcaoListaItemDto>> ExecutarAsync(
    string? cnpjEmpresa,
    CancellationToken cancellationToken = default)
  {
    string cnpj = SomenteDigitos(cnpjEmpresa);
    if (string.IsNullOrEmpty(cnpj))
    {
      return Array.Empty<PlanoRecepcaoListaItemDto>();
    }

    IReadOnlyList<PlanoRecepcao> entidades = await _planos.ListAsync(
      p => p.CnpjEmpresa == cnpj,
      cancellationToken);

    return entidades
      .OrderBy(p => p.NumeroPlano)
      .Select(PlanoRecepcaoListaItemDto.Mapear)
      .ToList();
  }

  private static string SomenteDigitos(string? valor)
  {
    if (string.IsNullOrWhiteSpace(valor))
    {
      return string.Empty;
    }

    return new string(valor.Where(char.IsDigit).ToArray());
  }
}
