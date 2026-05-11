using Suframa.RecepcaoRD.Application.DTOs.Cadsuf;

namespace Suframa.RecepcaoRD.Application.Features.Authentication;

public abstract record GenerateTokenOutcome;

public sealed record GenerateTokenOk(string Token, CadsufCompanyRegistrationVm? EmpresaCadsuf = null)
  : GenerateTokenOutcome;

public sealed record GenerateTokenBadRequest(string Message) : GenerateTokenOutcome;

public sealed record GenerateTokenNotFound(string Message) : GenerateTokenOutcome;
