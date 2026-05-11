namespace Suframa.RecepcaoRD.Domain.Entities;

public sealed class AuditoriaRdRecepcao
{
  private AuditoriaRdRecepcao()
  {
  }

  public static AuditoriaRdRecepcao Registrar(
    long idReferenciaRegistroAuditado,
    string cpfCnpjUsuarioSomenteDigitos,
    string nomeUsuario,
    string cnpjEmpresaRepresentadaSomenteDigitos,
    string nomeEmpresaRepresentada,
    string tipoAcao,
    DateTime dataHoraAcaoUtc,
    string descricaoAcao,
    int funcionalidadeId)
  {
    return new AuditoriaRdRecepcao
    {
      IdReferenciaRegistroAuditado = idReferenciaRegistroAuditado,
      CpfCnpjUsuario = cpfCnpjUsuarioSomenteDigitos,
      NomeUsuario = nomeUsuario,
      CnpjEmpresaRepresentada = cnpjEmpresaRepresentadaSomenteDigitos,
      NomeEmpresaRepresentada = nomeEmpresaRepresentada,
      TipoAcao = tipoAcao,
      DataHoraAcao = dataHoraAcaoUtc,
      DescricaoAcao = descricaoAcao,
      FuncionalidadeId = funcionalidadeId,
    };
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
