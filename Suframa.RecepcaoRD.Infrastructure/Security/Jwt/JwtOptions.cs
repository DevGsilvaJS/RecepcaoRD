namespace Suframa.RecepcaoRD.Infrastructure.Security.Jwt;

public sealed class JwtOptions
{
  public string TokenSecret { get; set; } = string.Empty;
  public int ExpirationMinutes { get; set; } = 30;
  public string Issuer { get; init; } = "RecepcaoRD";
  public string Audience { get; init; } = "RecepcaoRD";
}

