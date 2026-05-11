namespace Suframa.RecepcaoRD.Domain.Entities;

public sealed class PlanoRecepcao
{
  private PlanoRecepcao()
  {
  }

  public long Id { get; private set; }

  public string CnpjEmpresa { get; private set; } = string.Empty;

  public string NomeRepresentante { get; private set; } = string.Empty;

  public string CargoRepresentante { get; private set; } = string.Empty;

  public string EmailRepresentante { get; private set; } = string.Empty;

  public string FoneRepresentante { get; private set; } = string.Empty;

  public DateTime DataInicioVigencia { get; private set; }

  public DateTime DataFimVigencia { get; private set; }

  public string NumeroProtocolo { get; private set; } = string.Empty;

  public DateTime DataProtocolo { get; private set; }

  public string NumeroProcesso { get; private set; } = string.Empty;

  public string NumeroPlano { get; private set; } = string.Empty;

  public SituacaoPlanoRecepcao Situacao { get; private set; }

  public IReadOnlyCollection<RdRecepcao> Rds => _rds;

  private readonly List<RdRecepcao> _rds = new();
}
