namespace Suframa.RecepcaoRD.Application.Features.Authentication;

public interface IAuthenticationTokenService
{
  Task<GenerateTokenOutcome> GenerateTokenAsync(string? document, CancellationToken cancellationToken = default);
}
