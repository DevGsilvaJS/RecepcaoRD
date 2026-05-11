using Suframa.RecepcaoRD.Application.Features.Autenticacao;

namespace Suframa.RecepcaoRD.Application.Abstractions.Autenticacao;

public interface IIniciarSessaoAutenticacao
{
  Task<ResultadoIniciarSessaoAutenticacao> ExecutarAsync(
    string? documento,
    CancellationToken cancellationToken = default);
}
