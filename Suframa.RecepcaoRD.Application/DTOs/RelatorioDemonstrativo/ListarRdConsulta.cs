using Suframa.RecepcaoRD.Application.Abstractions.Persistence;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Application.DTOs.RelatorioDemonstrativo;

/// <summary>
/// Consulta da listagem: filtros de negócio + <see cref="PagedOptions"/> na mesma query string.
/// </summary>
public sealed class ListarRdConsulta : PagedOptions
{
  public string? Cnpj { get; set; }

  public string? AnoBase { get; set; }

  public string? SituacaoAtual { get; set; }

  /// <summary>
  /// Normaliza CNPJ e ano, mapeia nomes de ordenação da API para propriedades de <see cref="RdRecepcao"/>.
  /// </summary>
  /// <returns>CNPJ só com dígitos e ano de calendário opcional para o predicado.</returns>
  public (string CnpjSomenteDigitos, int? AnoCalendario) PrepararOrdenacaoEFiltroRd()
  {
    string cnpjSomenteDigitos = string.IsNullOrWhiteSpace(Cnpj)
      ? string.Empty
      : new string(Cnpj.Where(char.IsDigit).ToArray());

    int? anoCalendario = int.TryParse(AnoBase, out int ano) && ano > 0 ? ano : null;

    Sort = MapearCampoOrdenacaoRd(Sort);
    SortManny = SortManny?
      .Select(o => new SortOptions
      {
        Sort = MapearCampoOrdenacaoRd(o.Sort),
        Reverse = o.Reverse,
      })
      .ToList();

    return (cnpjSomenteDigitos, anoCalendario);
  }

  private static string MapearCampoOrdenacaoRd(string? campo)
  {
    string normalizado = (campo ?? string.Empty).Trim().ToLowerInvariant();
    return normalizado switch
    {
      "codigord" => nameof(RdRecepcao.NumeroDeclaracaoVeracidade),
      "tipord" => nameof(RdRecepcao.TipoRd),
      "anobase" => nameof(RdRecepcao.AnoCalendario),
      "razaosocial" => nameof(RdRecepcao.RazaoSocial),
      "cnpj" => nameof(RdRecepcao.Cnpj),
      "planoid" => nameof(RdRecepcao.PlanoId),
      "id" => nameof(RdRecepcao.Id),
      _ => string.IsNullOrEmpty(normalizado) ? string.Empty : nameof(RdRecepcao.Id),
    };
  }
}
