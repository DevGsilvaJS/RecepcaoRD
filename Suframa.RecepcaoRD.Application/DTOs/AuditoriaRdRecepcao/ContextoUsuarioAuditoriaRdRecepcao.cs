namespace Suframa.RecepcaoRD.Application.DTOs.AuditoriaRdRecepcao;

/// <summary>
/// Dados do usuário e da empresa representada para gravação em <c>SGT_AUDITORIA_RD_RECEPCAO</c>.
/// </summary>
public sealed record ContextoUsuarioAuditoriaRdRecepcao(
  string CpfCnpjUsuarioSomenteDigitos,
  string NomeUsuario,
  string CnpjEmpresaSomenteDigitos,
  string NomeEmpresa);
