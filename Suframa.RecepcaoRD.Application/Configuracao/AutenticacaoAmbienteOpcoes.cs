namespace Suframa.RecepcaoRD.Application.Configuracao;

public sealed class AutenticacaoAmbienteOpcoes
{
  public const string NomeSecao = "Autenticacao";

  public string CnpjFixoTeste { get; set; } = "11155708000138";

  public int ExpiracaoRefreshTokenEmDias { get; set; } = 7;
}
