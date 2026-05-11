using System.Text.Json.Serialization;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Application.DTOs.RelatorioDemonstrativo;

public sealed record RdRecepcaoListaItemDto(
  [property: JsonPropertyName("id")] long Id,
  [property: JsonPropertyName("cnpj")] string Cnpj,
  [property: JsonPropertyName("razaoSocial")] string RazaoSocial,
  [property: JsonPropertyName("anoBase")] int AnoBase,
  [property: JsonPropertyName("tipoRd")] int TipoRd,
  [property: JsonPropertyName("planoId")] long PlanoId,
  [property: JsonPropertyName("numeroPlano")] string NumeroPlano,
  [property: JsonPropertyName("numeroDeclaracaoVeracidade")] string? NumeroDeclaracaoVeracidade,
  [property: JsonPropertyName("representanteLegal")] string RepresentanteLegal,
  [property: JsonPropertyName("telefone")] string Telefone,
  [property: JsonPropertyName("email")] string Email,
  [property: JsonPropertyName("dataCriacaoUtc")] string DataCriacaoUtc)
{
  public static RdRecepcaoListaItemDto Mapear(RdRecepcao entidade)
  {
    string numeroPlano = entidade.Plano?.NumeroPlano ?? string.Empty;
    DateTime criacaoUtc = entidade.DataCriacao.Kind switch
    {
      DateTimeKind.Utc => entidade.DataCriacao,
      DateTimeKind.Local => entidade.DataCriacao.ToUniversalTime(),
      _ => DateTime.SpecifyKind(entidade.DataCriacao, DateTimeKind.Utc),
    };

    return new RdRecepcaoListaItemDto(
      entidade.Id,
      entidade.Cnpj,
      entidade.RazaoSocial,
      entidade.AnoCalendario,
      (int)entidade.TipoRd,
      entidade.PlanoId,
      numeroPlano,
      entidade.NumeroDeclaracaoVeracidade,
      entidade.RepresentanteLegal,
      entidade.Telefone,
      entidade.Email,
      criacaoUtc.ToString("o"));
  }
}
