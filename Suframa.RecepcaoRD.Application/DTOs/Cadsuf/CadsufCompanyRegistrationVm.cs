namespace Suframa.RecepcaoRD.Application.DTOs.Cadsuf;

public sealed record CadsufCompanyRegistrationVm(
  string Cnpj,
  string SuframaRegistration,
  string CompanyName,
  string Address);

