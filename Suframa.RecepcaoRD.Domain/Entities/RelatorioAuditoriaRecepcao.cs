namespace Suframa.RecepcaoRD.Domain.Entities;

public sealed class RelatorioAuditoriaRecepcao
{
  private RelatorioAuditoriaRecepcao()
  {
  }

  public long Id { get; private set; }

  public long RdRecepcaoId { get; private set; }

  public RdRecepcao RdRecepcao { get; private set; } = null!;

  public string RazaoSocialFirma { get; private set; } = string.Empty;

  public string CnpjFirma { get; private set; } = string.Empty;

  public DateTime DataRelatorio { get; private set; }

  public SituacaoRelatorioAuditoriaRecepcao Situacao { get; private set; }

  public string? Observacao { get; private set; }

  public long? IdPredecessor { get; private set; }

  public long? IdReferenciaAnalise { get; private set; }

  public byte? Alterado { get; private set; }
}
