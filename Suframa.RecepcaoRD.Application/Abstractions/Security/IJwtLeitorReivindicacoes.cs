using Suframa.RecepcaoRD.Application.DTOs.Seguranca;

namespace Suframa.RecepcaoRD.Application.Abstractions.Security;

/// <summary>
/// Lê reivindicações de um JWT já emitido pela aplicação (sem validar assinatura).
/// </summary>
public interface IJwtLeitorReivindicacoes
{
  ReivindicacoesJwtRecepcao? Ler(string? tokenJwt);
}
