using Suframa.RecepcaoRD.Application.DTOs.AuditoriaRdRecepcao;

namespace Suframa.RecepcaoRD.Application.Abstractions.AuditoriaRdRecepcao;

/// <summary>
/// Fornece o contexto do usuário autenticado na requisição atual (ex.: cookie JWT).
/// </summary>
public interface IContextoUsuarioAuditoriaRdRecepcao
{
  Task<ContextoUsuarioAuditoriaRdRecepcao?> ObterAsync(CancellationToken cancellationToken = default);
}
