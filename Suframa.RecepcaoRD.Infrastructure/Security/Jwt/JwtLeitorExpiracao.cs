using System.IdentityModel.Tokens.Jwt;
using Suframa.RecepcaoRD.Application.Abstractions.Security;

namespace Suframa.RecepcaoRD.Infrastructure.Security.Jwt;

public sealed class JwtLeitorExpiracao : IJwtLeitorExpiracao
{
  public DateTimeOffset? ObterExpiracaoJwtUtc(string tokenJwt)
  {
    try
    {
      JwtSecurityToken token = new JwtSecurityTokenHandler().ReadJwtToken(tokenJwt);
      if (token.ValidTo == DateTime.MinValue)
      {
        return null;
      }

      return new DateTimeOffset(DateTime.SpecifyKind(token.ValidTo, DateTimeKind.Utc));
    }
    catch
    {
      return null;
    }
  }
}
