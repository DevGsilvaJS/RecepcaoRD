namespace Suframa.RecepcaoRD.Domain.Entities;

public enum SituacaoPlanoRecepcao
{
  Entregue = 1,
  AguardandoAnalise = 2,
  EmAnalise = 3,
  EmRevisaoPelaEmpresa = 4,
  Deferido = 5,
  Indeferido = 6,
}
