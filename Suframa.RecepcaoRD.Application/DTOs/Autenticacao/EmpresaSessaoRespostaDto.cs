using System.Text.Json.Serialization;
using Suframa.RecepcaoRD.Application.DTOs.Cadsuf;

namespace Suframa.RecepcaoRD.Application.DTOs.Autenticacao;

public sealed record EmpresaSessaoRespostaDto(
  [property: JsonPropertyName("cnpj")] string Cnpj,
  [property: JsonPropertyName("inscricaoSuframa")] string InscricaoSuframa,
  [property: JsonPropertyName("razaoSocial")] string RazaoSocial,
  [property: JsonPropertyName("endereco")] string Endereco)
{
  public static EmpresaSessaoRespostaDto MapearDaEmpresaCadsuf(CadsufCompanyRegistrationVm empresa)
  {
    return new EmpresaSessaoRespostaDto(
      empresa.Cnpj,
      empresa.SuframaRegistration,
      empresa.CompanyName,
      empresa.Address);
  }
}
