namespace Suframa.RecepcaoRD.Application.Abstractions.Security;

public interface IJwtTokenGenerator
{
  string GenerateToken(
    string document,
    string? cnpj = null,
    string? cpf = null,
    string? companyName = null,
    string? nomeUsuario = null);
}

