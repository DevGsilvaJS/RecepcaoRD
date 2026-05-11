using Suframa.RecepcaoRD.Application.Features.Autenticacao;

namespace Suframa.RecepcaoRD.Application.Abstractions.Autenticacao;

public interface IObterSituacaoSessaoAutenticacao
{
  SituacaoSessaoAutenticacaoSaida Executar(string? tokenJwt);
}
