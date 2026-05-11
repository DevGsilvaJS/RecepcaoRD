namespace Suframa.RecepcaoRD.Domain.Entities;

public sealed class TiposAnexosRecepcao
{
  private TiposAnexosRecepcao()
  {
  }

  public long Id { get; private set; }

  public string Nome { get; private set; } = string.Empty;

  public IReadOnlyCollection<AnexosRecepcao> Anexos => _anexos;

  private readonly List<AnexosRecepcao> _anexos = new();
}
