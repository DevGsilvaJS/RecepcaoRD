namespace Suframa.RecepcaoRD.Application.Abstractions.Autenticacao;

public interface IEncerrarSessaoAutenticacao
{
  void Executar(string? refreshToken);
}
