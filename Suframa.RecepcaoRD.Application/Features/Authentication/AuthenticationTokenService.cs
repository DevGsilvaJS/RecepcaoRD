using Suframa.RecepcaoRD.Application.Abstractions.Cadsuf;
using Suframa.RecepcaoRD.Application.Abstractions.Security;

namespace Suframa.RecepcaoRD.Application.Features.Authentication;

public sealed class AuthenticationTokenService : IAuthenticationTokenService
{
  private readonly ICadsufCompanyRepository _cadsufCompanyRepository;
  private readonly IJwtTokenGenerator _jwtTokenGenerator;

  public AuthenticationTokenService(
    ICadsufCompanyRepository cadsufCompanyRepository,
    IJwtTokenGenerator jwtTokenGenerator)
  {
    _cadsufCompanyRepository = cadsufCompanyRepository;
    _jwtTokenGenerator = jwtTokenGenerator;
  }

  public async Task<GenerateTokenOutcome> GenerateTokenAsync(string? document, CancellationToken cancellationToken = default)
  {
    string normalized = NormalizeDigits(document);
    if (normalized.Length is not (11 or 14))
    {
      return new GenerateTokenBadRequest(
        "O parâmetro 'document' deve conter um CPF (11 dígitos) ou CNPJ (14 dígitos).");
    }

    if (normalized.Length == 14)
    {
      var company = await _cadsufCompanyRepository.GetCompanyByCnpjAsync(normalized, cancellationToken);
      if (company is null)
      {
        return new GenerateTokenNotFound("Empresa não encontrada no CADSUF para o CNPJ informado.");
      }

      string token = _jwtTokenGenerator.GenerateToken(
        document: normalized,
        cnpj: company.Cnpj,
        companyName: company.CompanyName,
        nomeUsuario: company.CompanyName);

      return new GenerateTokenOk(token, company);
    }

    var empresaPorCpf = await _cadsufCompanyRepository.GetCompanyByCpfAsync(normalized, cancellationToken);
    if (empresaPorCpf is null)
    {
      return new GenerateTokenNotFound("Nenhuma empresa encontrada no CADSUF para o CPF informado.");
    }

    string cpfToken = _jwtTokenGenerator.GenerateToken(
      document: normalized,
      cnpj: empresaPorCpf.Cnpj,
      cpf: normalized,
      companyName: empresaPorCpf.CompanyName,
      nomeUsuario: empresaPorCpf.CompanyName);

    return new GenerateTokenOk(cpfToken, empresaPorCpf);
  }

  private static string NormalizeDigits(string? value)
  {
    if (string.IsNullOrWhiteSpace(value))
    {
      return string.Empty;
    }

    return new string(value.Where(char.IsDigit).ToArray());
  }
}
