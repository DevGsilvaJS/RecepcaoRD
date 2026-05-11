namespace Suframa.RecepcaoRD.Domain.Entities;

public sealed class ListaFuncionalidadeRecepcao
{
  private ListaFuncionalidadeRecepcao()
  {
  }

  public int Id { get; private set; }

  public string Descricao { get; private set; } = string.Empty;

  public string NomeTabela { get; private set; } = string.Empty;

  public int? IdVinculoFuncionalidadePai { get; private set; }

  public ListaFuncionalidadeRecepcao? FuncionalidadePai { get; private set; }

  public IReadOnlyCollection<ListaFuncionalidadeRecepcao> FuncionalidadesFilhas => _filhas;

  private readonly List<ListaFuncionalidadeRecepcao> _filhas = new();

  public IReadOnlyCollection<AuditoriaRdRecepcao> Auditorias => _auditorias;

  private readonly List<AuditoriaRdRecepcao> _auditorias = new();

  public IReadOnlyCollection<AnexosRecepcao> Anexos => _anexos;

  private readonly List<AnexosRecepcao> _anexos = new();
}
