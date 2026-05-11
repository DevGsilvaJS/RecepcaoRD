namespace Suframa.RecepcaoRD.Domain.Entities;

public sealed class RdRecepcao
{
  private RdRecepcao()
  {
  }

  public static RdRecepcao Criar(
    string cnpjSomenteDigitos,
    string razaoSocial,
    string enderecoEmpresa,
    string inscricaoSuframa,
    int anoCalendario,
    TipoRdRecepcao tipoRd,
    long planoId,
    string representanteLegal,
    string telefone,
    string email,
    long? idOrigem)
  {
    DateTime agoraUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
    return new RdRecepcao
    {
      Cnpj = cnpjSomenteDigitos,
      RazaoSocial = razaoSocial,
      TipoRd = tipoRd,
      IdOrigem = idOrigem,
      InscricaoSuframa = inscricaoSuframa,
      EnderecoEmpresa = enderecoEmpresa,
      RepresentanteLegal = representanteLegal,
      Telefone = telefone,
      Email = email,
      AnoCalendario = anoCalendario,
      PlanoId = planoId,
      NumeroDeclaracaoVeracidade = null,
      DataEmissao = null,
      DataCriacao = agoraUtc,
    };
  }

  public void AtualizarCadastro(
    int anoCalendario,
    long planoId,
    string representanteLegal,
    string telefone,
    string email)
  {
    AnoCalendario = anoCalendario;
    PlanoId = planoId;
    RepresentanteLegal = representanteLegal;
    Telefone = telefone;
    Email = email;
  }

  public long Id { get; private set; }

  public string Cnpj { get; private set; } = string.Empty;

  public string RazaoSocial { get; private set; } = string.Empty;

  public TipoRdRecepcao TipoRd { get; private set; }

  public long? IdOrigem { get; private set; }

  public RdRecepcao? RdOrigem { get; private set; }

  public string InscricaoSuframa { get; private set; } = string.Empty;

  public string EnderecoEmpresa { get; private set; } = string.Empty;

  public string RepresentanteLegal { get; private set; } = string.Empty;

  public string Telefone { get; private set; } = string.Empty;

  public string Email { get; private set; } = string.Empty;

  public int AnoCalendario { get; private set; }

  public long PlanoId { get; private set; }

  public PlanoRecepcao Plano { get; private set; } = null!;

  public string? NumeroDeclaracaoVeracidade { get; private set; }

  public DateTime? DataEmissao { get; private set; }

  public DateTime DataCriacao { get; private set; }

  public IReadOnlyCollection<RelatorioAuditoriaRecepcao> RelatoriosAuditoria => _relatoriosAuditoria;

  private readonly List<RelatorioAuditoriaRecepcao> _relatoriosAuditoria = new();
}
