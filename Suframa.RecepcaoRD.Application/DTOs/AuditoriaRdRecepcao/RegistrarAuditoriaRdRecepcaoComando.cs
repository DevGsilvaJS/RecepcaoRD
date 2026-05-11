namespace Suframa.RecepcaoRD.Application.DTOs.AuditoriaRdRecepcao;

/// <summary>
/// Parâmetros de negócio para registrar uma linha de auditoria (I/A/E).
/// </summary>
public sealed record RegistrarAuditoriaRdRecepcaoComando(
  long IdReferencia,
  string TipoAcao,
  string DescricaoAcao,
  int FuncionalidadeId);
