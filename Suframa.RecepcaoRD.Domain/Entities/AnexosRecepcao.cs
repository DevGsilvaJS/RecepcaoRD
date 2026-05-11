namespace Suframa.RecepcaoRD.Domain.Entities;

public sealed class AnexosRecepcao
{
  private AnexosRecepcao()
  {
  }

  public long Id { get; private set; }

  public long IdReferenciaRegistroPai { get; private set; }

  public long TipoAnexoId { get; private set; }

  public TiposAnexosRecepcao TipoAnexo { get; private set; } = null!;

  public int FuncionalidadeId { get; private set; }

  public ListaFuncionalidadeRecepcao Funcionalidade { get; private set; } = null!;

  public string NomeArquivo { get; private set; } = string.Empty;

  public byte[]? Arquivo { get; private set; }
}
