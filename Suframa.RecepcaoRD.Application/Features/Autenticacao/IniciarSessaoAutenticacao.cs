using Suframa.RecepcaoRD.Application.Abstractions.Autenticacao;
using Suframa.RecepcaoRD.Application.Abstractions.Security;
using Suframa.RecepcaoRD.Application.DTOs.Autenticacao;
using Suframa.RecepcaoRD.Application.Features.Authentication;

namespace Suframa.RecepcaoRD.Application.Features.Autenticacao;

public sealed class IniciarSessaoAutenticacao : IIniciarSessaoAutenticacao
{
  private readonly IAuthenticationTokenService _tokens;
  private readonly IArmazenamentoRefreshToken _refreshTokens;
  private readonly IJwtLeitorExpiracao _jwtLeitor;

  public IniciarSessaoAutenticacao(
    IAuthenticationTokenService tokens,
    IArmazenamentoRefreshToken refreshTokens,
    IJwtLeitorExpiracao jwtLeitor)
  {
    _tokens = tokens;
    _refreshTokens = refreshTokens;
    _jwtLeitor = jwtLeitor;
  }

  public async Task<ResultadoIniciarSessaoAutenticacao> ExecutarAsync(
    string? documentoInformado,
    CancellationToken cancellationToken = default)
  {
    string documento = NormalizarDigitos(documentoInformado);
    if (string.IsNullOrEmpty(documento))
    {
      return new IniciarSessaoAutenticacaoFalhaGeracaoToken(
        "Informe o CPF (11 dígitos) ou o CNPJ (14 dígitos).");
    }

    if (documento.Length is not (11 or 14))
    {
      return new IniciarSessaoAutenticacaoFalhaGeracaoToken(
        "O documento deve conter um CPF (11 dígitos) ou CNPJ (14 dígitos).");
    }

    GenerateTokenOutcome geracao = await _tokens.GenerateTokenAsync(documento, cancellationToken);
    if (geracao is GenerateTokenBadRequest malFormado)
    {
      return new IniciarSessaoAutenticacaoFalhaGeracaoToken(malFormado.Message);
    }

    if (geracao is GenerateTokenNotFound naoEncontrado)
    {
      return new IniciarSessaoAutenticacaoEmpresaInexistente(naoEncontrado.Message);
    }

    if (geracao is not GenerateTokenOk ok)
    {
      return new IniciarSessaoAutenticacaoFalhaGeracaoToken("Não foi possível emitir o token de acesso.");
    }

    DateTimeOffset? expiraJwt = _jwtLeitor.ObterExpiracaoJwtUtc(ok.Token);
    if (expiraJwt is null)
    {
      return new IniciarSessaoAutenticacaoFalhaGeracaoToken("Token emitido sem data de expiração válida.");
    }

    EmpresaSessaoRespostaDto? dtoEmpresa = ok.EmpresaCadsuf is null
      ? null
      : EmpresaSessaoRespostaDto.MapearDaEmpresaCadsuf(ok.EmpresaCadsuf);

    RefreshTokenEmitido refresh = _refreshTokens.EmitirParaDocumento(documento);
    var saida = new IniciarSessaoAutenticacaoSaida(
      ok.Token,
      expiraJwt.Value,
      refresh.Token,
      refresh.ExpiraEmUtc,
      "Sessão iniciada com sucesso (JWT 30 min + refresh token).",
      dtoEmpresa);

    return new IniciarSessaoAutenticacaoSucesso(saida);
  }

  private static string NormalizarDigitos(string? valor)
  {
    if (string.IsNullOrWhiteSpace(valor))
    {
      return string.Empty;
    }

    return new string(valor.Where(char.IsDigit).ToArray());
  }
}
