namespace Suframa.RecepcaoRD.Application.Abstractions.Persistence;

/// <summary>
/// Uma ordenação na lista (equivalente ao legado <c>SortOptions</c>).
/// </summary>
public sealed class SortOptions
{
  public string? Sort { get; set; }

  public bool Reverse { get; set; }
}

/// <summary>
/// Opções de paginação e ordenação vindas da query string (equivalente ao legado <c>PagedOptions</c>).
/// </summary>
public class PagedOptions
{
  public int? Page { get; set; } = 1;

  public bool Reverse { get; set; }

  public int? Size { get; set; } = 10;

  public string? Sort { get; set; }

  /// <summary>
  /// Limite de registros na busca (equivalente a <c>RegLimited</c>).
  /// </summary>
  public int? RegLimited { get; set; }

  public IEnumerable<SortOptions>? SortManny { get; set; }
}
