namespace Suframa.RecepcaoRD.Domain.Entities;

public sealed class AuditoriaRdRecepcao
{
  private AuditoriaRdRecepcao()
  {
  }

  public long Id { get; private set; }

  public long IdReferenciaRegistroAuditado { get; private set; }

  public string CpfCnpjUsuario { get; private set; } = string.Empty;

  public string NomeUsuario { get; private set; } = string.Empty;

  public string CnpjEmpresaRepresentada { get; private set; } = string.Empty;

  public string NomeEmpresaRepresentada { get; private set; } = string.Empty;

  public string TipoAcao { get; private set; } = string.Empty;

  public DateTime DataHoraAcao { get; private set; }

  public string DescricaoAcao { get; private set; } = string.Empty;

  public int FuncionalidadeId { get; private set; }

  public ListaFuncionalidadeRecepcao Funcionalidade { get; private set; } = null!;
}
