using System.IdentityModel.Tokens.Jwt;
using Suframa.RecepcaoRD.Application.Abstractions.Security;
using Suframa.RecepcaoRD.Application.DTOs.Seguranca;

namespace Suframa.RecepcaoRD.Infrastructure.Security.Jwt;

public sealed class JwtLeitorReivindicacoes : IJwtLeitorReivindicacoes
{
  public ReivindicacoesJwtRecepcao? Ler(string? tokenJwt)
  {
    if (string.IsNullOrWhiteSpace(tokenJwt))
    {
      return null;
    }

    try
    {
      JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(tokenJwt);
      string? documento = ObterPrimeiraReivindicacao(token, "document", JwtRegisteredClaimNames.Sub);
      if (string.IsNullOrWhiteSpace(documento))
      {
        return null;
      }

      string? cpf = ObterPrimeiraReivindicacao(token, "cpf");
      string? cnpj = ObterPrimeiraReivindicacao(token, "cnpj");
      string? nomeEmpresa = ObterPrimeiraReivindicacao(token, "companyName");
      string? nomeUsuario = ObterPrimeiraReivindicacao(token, "nomeUsuario");
      return new ReivindicacoesJwtRecepcao(documento.Trim(), cpf, cnpj, nomeEmpresa, nomeUsuario);
    }
    catch
    {
      return null;
    }
  }

  private static string? ObterPrimeiraReivindicacao(JwtSecurityToken token, params string[] tipos)
  {
    foreach (string tipo in tipos)
    {
      string? valor = token.Claims.FirstOrDefault(c => c.Type == tipo)?.Value;
      if (!string.IsNullOrWhiteSpace(valor))
      {
        return valor.Trim();
      }
    }

    return null;
  }
}
