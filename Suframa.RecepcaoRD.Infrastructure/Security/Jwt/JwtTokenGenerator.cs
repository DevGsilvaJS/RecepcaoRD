using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Suframa.RecepcaoRD.Application.Abstractions.Security;

namespace Suframa.RecepcaoRD.Infrastructure.Security.Jwt;

public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
  private readonly JwtOptions _options;

  public JwtTokenGenerator(IOptions<JwtOptions> options)
  {
    _options = options.Value;
  }

  public string GenerateToken(
    string document,
    string? cnpj = null,
    string? cpf = null,
    string? companyName = null,
    string? nomeUsuario = null)
  {
    byte[] secretBytes = Convert.FromBase64String(_options.TokenSecret);
    var signingKey = new SymmetricSecurityKey(secretBytes);

    var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

    List<Claim> claims =
    [
      new(JwtRegisteredClaimNames.Sub, document),
      new("document", document),
    ];

    if (!string.IsNullOrWhiteSpace(cnpj))
    {
      claims.Add(new Claim("cnpj", cnpj));
    }

    if (!string.IsNullOrWhiteSpace(cpf))
    {
      claims.Add(new Claim("cpf", cpf));
    }

    if (!string.IsNullOrWhiteSpace(companyName))
    {
      claims.Add(new Claim("companyName", companyName));
    }

    string nomeParaReivindicacao = !string.IsNullOrWhiteSpace(nomeUsuario)
      ? nomeUsuario.Trim()
      : (companyName ?? string.Empty).Trim();

    if (nomeParaReivindicacao.Length > 0)
    {
      claims.Add(new Claim("nomeUsuario", nomeParaReivindicacao));
    }

    DateTime nowUtc = DateTime.UtcNow;
    DateTime expires = nowUtc.AddMinutes(_options.ExpirationMinutes);

    var token = new JwtSecurityToken(
      issuer: _options.Issuer,
      audience: _options.Audience,
      claims: claims,
      notBefore: nowUtc,
      expires: expires,
      signingCredentials: credentials);

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}

