using System.Text.Json.Serialization;

namespace Suframa.RecepcaoRD.Application.Abstractions.Persistence;

/// <summary>
/// Resultado de listagem paginada (itens da página e total de registros).
/// </summary>
public sealed class PagedItems<T>
{
  public PagedItems(IReadOnlyList<T> items, int total)
  {
    Items = items;
    Total = total;
  }

  [JsonPropertyName("items")]
  public IReadOnlyList<T> Items { get; }

  [JsonPropertyName("total")]
  public int Total { get; }
}
