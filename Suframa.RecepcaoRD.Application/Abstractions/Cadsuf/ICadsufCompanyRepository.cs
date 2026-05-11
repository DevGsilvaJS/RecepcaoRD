using Suframa.RecepcaoRD.Application.DTOs.Cadsuf;

namespace Suframa.RecepcaoRD.Application.Abstractions.Cadsuf;

public interface ICadsufCompanyRepository
{
  Task<CadsufCompanyRegistrationVm?> GetCompanyByCnpjAsync(string cnpj, CancellationToken cancellationToken = default);

  Task<CadsufCompanyRegistrationVm?> GetCompanyByCpfAsync(string cpfSomenteDigitos, CancellationToken cancellationToken = default);
}

