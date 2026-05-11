using Suframa.RecepcaoRD.Application.DTOs.AuditoriaRdRecepcao;

namespace Suframa.RecepcaoRD.Application.Abstractions.AuditoriaRdRecepcao;

/// <summary>
/// Registra ações de inclusão, alteração e exclusão na tabela <c>SGT_AUDITORIA_RD_RECEPCAO</c>.
/// </summary>
public interface IRegistrarAuditoriaRdRecepcao
{
  /// <summary>
  /// Usa <see cref="IContextoUsuarioAuditoriaRdRecepcao"/> (sessão HTTP típica).
  /// </summary>
  Task RegistrarAsync(RegistrarAuditoriaRdRecepcaoComando comando, CancellationToken cancellationToken = default);

  /// <summary>
  /// Permite informar o contexto explicitamente (jobs, integrações, testes).
  /// </summary>
  Task RegistrarAsync(
    RegistrarAuditoriaRdRecepcaoComando comando,
    ContextoUsuarioAuditoriaRdRecepcao contextoUsuario,
    CancellationToken cancellationToken = default);
}
