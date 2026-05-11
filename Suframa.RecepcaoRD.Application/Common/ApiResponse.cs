namespace Suframa.RecepcaoRD.Application.Common;

/// <summary>
/// Envelope padrão de resposta da API.
/// </summary>
public sealed class ApiResponse<T>
{
  public bool Sucesso { get; init; }

  public string? Mensagem { get; init; }

  public T? Dados { get; init; }

  public IReadOnlyList<string>? Erros { get; init; }
}
