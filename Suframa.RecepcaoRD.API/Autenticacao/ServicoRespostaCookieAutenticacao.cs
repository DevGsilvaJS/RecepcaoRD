using Microsoft.AspNetCore.Http;

namespace Suframa.RecepcaoRD.API.Autenticacao;

public sealed class ServicoRespostaCookieAutenticacao
{
  private const string NomeCookieJwt = "sagat_jwt";
  private const string NomeCookieRefresh = "sagat_refresh";

  private readonly IHttpContextAccessor _acessoHttp;

  public ServicoRespostaCookieAutenticacao(IHttpContextAccessor acessoHttp)
  {
    _acessoHttp = acessoHttp;
  }

  public void DefinirCookieJwt(string tokenJwt, DateTimeOffset expiraEmUtc)
  {
    HttpContext contexto = ObterContexto();
    contexto.Response.Cookies.Append(
      NomeCookieJwt,
      tokenJwt,
      CriarOpcoesCookie(contexto, expiraEmUtc));
  }

  public void DefinirCookieRefreshToken(string refreshToken, DateTimeOffset expiraEmUtc)
  {
    HttpContext contexto = ObterContexto();
    contexto.Response.Cookies.Append(
      NomeCookieRefresh,
      refreshToken,
      CriarOpcoesCookie(contexto, expiraEmUtc));
  }

  public void RemoverCookiesDeSessao()
  {
    HttpContext contexto = ObterContexto();
    RemoverCookie(contexto, NomeCookieJwt);
    RemoverCookie(contexto, NomeCookieRefresh);
  }

  private static void RemoverCookie(HttpContext contexto, string nome)
  {
    contexto.Response.Cookies.Append(nome, string.Empty, CriarOpcoesCookie(contexto, DateTimeOffset.UnixEpoch));
  }

  private static CookieOptions CriarOpcoesCookie(HttpContext contexto, DateTimeOffset expiraEm)
  {
    SameSiteMode mesmoSite = contexto.Request.IsHttps ? SameSiteMode.None : SameSiteMode.Lax;

    return new CookieOptions
    {
      HttpOnly = true,
      Secure = contexto.Request.IsHttps,
      SameSite = mesmoSite,
      Expires = expiraEm,
    };
  }

  private HttpContext ObterContexto()
  {
    return _acessoHttp.HttpContext
      ?? throw new InvalidOperationException("HttpContext não está disponível.");
  }
}
