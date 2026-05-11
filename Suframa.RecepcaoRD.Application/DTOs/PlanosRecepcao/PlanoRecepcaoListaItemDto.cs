using System.Text.Json.Serialization;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Application.DTOs.PlanosRecepcao;

public sealed record PlanoRecepcaoListaItemDto(
  [property: JsonPropertyName("id")] long Id,
  [property: JsonPropertyName("cnpjEmpresa")] string CnpjEmpresa,
  [property: JsonPropertyName("nomeRepresentante")] string NomeRepresentante,
  [property: JsonPropertyName("cargoRepresentante")] string CargoRepresentante,
  [property: JsonPropertyName("emailRepresentante")] string EmailRepresentante,
  [property: JsonPropertyName("foneRepresentante")] string FoneRepresentante,
  [property: JsonPropertyName("dataInicioVigencia")] DateTime DataInicioVigencia,
  [property: JsonPropertyName("dataFimVigencia")] DateTime DataFimVigencia,
  [property: JsonPropertyName("numeroProtocolo")] string NumeroProtocolo,
  [property: JsonPropertyName("dataProtocolo")] DateTime DataProtocolo,
  [property: JsonPropertyName("numeroProcesso")] string NumeroProcesso,
  [property: JsonPropertyName("numeroPlano")] string NumeroPlano,
  [property: JsonPropertyName("situacao")] SituacaoPlanoRecepcao Situacao)
{
  public static PlanoRecepcaoListaItemDto Mapear(PlanoRecepcao entidade)
  {
    return new PlanoRecepcaoListaItemDto(
      entidade.Id,
      entidade.CnpjEmpresa,
      entidade.NomeRepresentante,
      entidade.CargoRepresentante,
      entidade.EmailRepresentante,
      entidade.FoneRepresentante,
      entidade.DataInicioVigencia,
      entidade.DataFimVigencia,
      entidade.NumeroProtocolo,
      entidade.DataProtocolo,
      entidade.NumeroProcesso,
      entidade.NumeroPlano,
      entidade.Situacao);
  }
}
