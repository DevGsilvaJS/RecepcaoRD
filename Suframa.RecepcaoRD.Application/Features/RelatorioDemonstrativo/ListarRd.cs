using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Suframa.RecepcaoRD.Application.Abstractions.Persistence;
using Suframa.RecepcaoRD.Application.Abstractions.RelatorioDemonstrativo;
using Suframa.RecepcaoRD.Application.DTOs.RelatorioDemonstrativo;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Application.Features.RelatorioDemonstrativo;

public sealed class ListarRd : IListarRd
{
  private readonly IRepository<RdRecepcao> _rds;

  public ListarRd(IRepository<RdRecepcao> rds)
  {
    _rds = rds;
  }

  public async Task<PagedItems<RdRecepcaoListaItemDto>> ListarAsync(
    ListarRdConsulta consulta,
    CancellationToken cancellationToken = default)
  {
    (string cnpj, int? ano) = consulta.PrepararOrdenacaoEFiltroRd();
    if (string.IsNullOrEmpty(cnpj))
    {
      return new PagedItems<RdRecepcaoListaItemDto>(Array.Empty<RdRecepcaoListaItemDto>(), 0);
    }

    Expression<Func<RdRecepcao, bool>> predicado = r =>
      r.Cnpj == cnpj && (!ano.HasValue || r.AnoCalendario == ano.Value);

    PagedItems<RdRecepcao> pagina = await _rds.ListarPaginadoAsync(
      predicado,
      consulta,
      q => q.Include(r => r.Plano),
      cancellationToken);

    IReadOnlyList<RdRecepcaoListaItemDto> linhas = pagina.Items
      .Select(RdRecepcaoListaItemDto.Mapear)
      .ToList();

    return new PagedItems<RdRecepcaoListaItemDto>(linhas, pagina.Total);
  }
}
