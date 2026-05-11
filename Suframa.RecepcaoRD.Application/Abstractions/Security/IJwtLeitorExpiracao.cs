namespace Suframa.RecepcaoRD.Application.Abstractions.Security;

public interface IJwtLeitorExpiracao
{
  DateTimeOffset? ObterExpiracaoJwtUtc(string tokenJwt);
}
