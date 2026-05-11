using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Suframa.RecepcaoRD.API.Autenticacao;
using Suframa.RecepcaoRD.Application.Abstractions.Autenticacao;
using Suframa.RecepcaoRD.Application.Features.Autenticacao;

namespace Suframa.RecepcaoRD.API.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/autenticacao")]
public sealed class AutenticacaoController : ControllerBase
{
  private const string NomeCookieJwt = "sagat_jwt";
  private const string NomeCookieRefresh = "sagat_refresh";

  private readonly IIniciarSessaoAutenticacao _iniciarSessao;
  private readonly IRenovarSessaoAutenticacao _renovarSessao;
  private readonly IEncerrarSessaoAutenticacao _encerrarSessao;
  private readonly IObterSituacaoSessaoAutenticacao _obterSituacaoSessao;
  private readonly ServicoRespostaCookieAutenticacao _cookies;

  public AutenticacaoController(
    IIniciarSessaoAutenticacao iniciarSessao,
    IRenovarSessaoAutenticacao renovarSessao,
    IEncerrarSessaoAutenticacao encerrarSessao,
    IObterSituacaoSessaoAutenticacao obterSituacaoSessao,
    ServicoRespostaCookieAutenticacao cookies)
  {
    _iniciarSessao = iniciarSessao;
    _renovarSessao = renovarSessao;
    _encerrarSessao = encerrarSessao;
    _obterSituacaoSessao = obterSituacaoSessao;
    _cookies = cookies;
  }

  [HttpGet]
  public async Task<IActionResult> Autenticar(
    [FromQuery(Name = "document")] string? document,
    CancellationToken cancellationToken)
  {
    if (string.IsNullOrWhiteSpace(document))
    {
      return BadRequest("Informe o CPF ou o CNPJ no parâmetro de consulta 'document'.");
    }

    ResultadoIniciarSessaoAutenticacao resultado = await _iniciarSessao.ExecutarAsync(document, cancellationToken);
    return resultado switch
    {
      IniciarSessaoAutenticacaoSucesso sucesso => GravarCookiesInicio(sucesso.Saida),
      IniciarSessaoAutenticacaoEmpresaInexistente naoEncontrada => NotFound(naoEncontrada.Mensagem),
      IniciarSessaoAutenticacaoFalhaGeracaoToken falha => BadRequest(falha.Mensagem),
      _ => Problem(statusCode: StatusCodes.Status500InternalServerError),
    };
  }

  [HttpPost("refresh")]
  public async Task<IActionResult> Renovar(CancellationToken cancellationToken)
  {
    Request.Cookies.TryGetValue(NomeCookieRefresh, out string? refreshToken);
    ResultadoRenovarSessaoAutenticacao resultado = await _renovarSessao.ExecutarAsync(refreshToken, cancellationToken);
    if (resultado is not RenovarSessaoAutenticacaoSucesso sucesso)
    {
      return Unauthorized();
    }

    _cookies.DefinirCookieJwt(sucesso.Saida.TokenJwt, sucesso.Saida.ExpiraEmJwtUtc);
    _cookies.DefinirCookieRefreshToken(sucesso.Saida.RefreshToken, sucesso.Saida.ExpiraRefreshUtc);

    return Ok(new
    {
      mensagem = sucesso.Saida.Mensagem,
      expiraEmJwtUtc = sucesso.Saida.ExpiraEmJwtUtc,
    });
  }

  [HttpPost("logout")]
  public IActionResult Sair()
  {
    Request.Cookies.TryGetValue(NomeCookieRefresh, out string? refreshToken);
    _encerrarSessao.Executar(refreshToken);
    _cookies.RemoverCookiesDeSessao();
    return Ok(new { mensagem = "Sessão encerrada." });
  }

  [HttpGet("sessao")]
  public IActionResult ObterSessao()
  {
    Request.Cookies.TryGetValue(NomeCookieJwt, out string? tokenJwt);
    SituacaoSessaoAutenticacaoSaida situacao = _obterSituacaoSessao.Executar(tokenJwt);

    // 200 + autenticado=false evita 401 no navegador na primeira visita e não confunde com falha de API protegida.
    return Ok(new
    {
      autenticado = situacao.Autenticado,
      expiraEmJwtUtc = situacao.Autenticado ? situacao.ExpiraEmJwtUtc : null,
    });
  }

  private IActionResult GravarCookiesInicio(IniciarSessaoAutenticacaoSaida saida)
  {
    _cookies.DefinirCookieJwt(saida.TokenJwt, saida.ExpiraEmJwtUtc);
    _cookies.DefinirCookieRefreshToken(saida.RefreshToken, saida.ExpiraRefreshUtc);

    return Ok(new
    {
      mensagem = saida.Mensagem,
      expiraEmJwtUtc = saida.ExpiraEmJwtUtc,
      empresa = saida.Empresa,
    });
  }
}
