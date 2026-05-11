using Microsoft.AspNetCore.Http;
using Suframa.RecepcaoRD.Application.Abstractions.AuditoriaRdRecepcao;
using Suframa.RecepcaoRD.Application.Abstractions.Security;
using Suframa.RecepcaoRD.Application.DTOs.AuditoriaRdRecepcao;
using Suframa.RecepcaoRD.Application.DTOs.Seguranca;

namespace Suframa.RecepcaoRD.Infrastructure.Auditoria;

/// <summary>
/// Adaptador HTTP: lê o JWT do cookie de sessão e monta o contexto de auditoria.
/// CNPJ e nome da empresa vêm das claims <c>cnpj</c> e <c>companyName</c> preenchidas na autenticação (CADSUF);
/// documento do usuário de <c>cpf</c> ou <c>document</c>; nome de exibição de <c>nomeUsuario</c> (ou empresa).
/// </summary>
public sealed class ContextoUsuarioAuditoriaRdRecepcaoHttp : IContextoUsuarioAuditoriaRdRecepcao
{
  private const string NomeCookieJwt = "sagat_jwt";
  private const int TamanhoMaximoCpfCnpj = 14;
  private const int TamanhoMaximoNome = 155;

  private readonly IHttpContextAccessor _http;
  private readonly IJwtLeitorExpiracao _expiracaoJwt;
  private readonly IJwtLeitorReivindicacoes _reivindicacoesJwt;

  public ContextoUsuarioAuditoriaRdRecepcaoHttp(
    IHttpContextAccessor http,
    IJwtLeitorExpiracao expiracaoJwt,
    IJwtLeitorReivindicacoes reivindicacoesJwt)
  {
    _http = http;
    _expiracaoJwt = expiracaoJwt;
    _reivindicacoesJwt = reivindicacoesJwt;
  }

  public Task<ContextoUsuarioAuditoriaRdRecepcao?> ObterAsync(CancellationToken cancellationToken = default)
  {
    HttpContext? contexto = _http.HttpContext;
    if (contexto is null)
    {
      return Task.FromResult<ContextoUsuarioAuditoriaRdRecepcao?>(null);
    }

    contexto.Request.Cookies.TryGetValue(NomeCookieJwt, out string? jwt);
    if (string.IsNullOrWhiteSpace(jwt))
    {
      return Task.FromResult<ContextoUsuarioAuditoriaRdRecepcao?>(null);
    }

    DateTimeOffset? expira = _expiracaoJwt.ObterExpiracaoJwtUtc(jwt);
    if (expira is null || expira <= DateTimeOffset.UtcNow)
    {
      return Task.FromResult<ContextoUsuarioAuditoriaRdRecepcao?>(null);
    }

    ReivindicacoesJwtRecepcao? reivindicacoes = _reivindicacoesJwt.Ler(jwt);
    if (reivindicacoes is null)
    {
      return Task.FromResult<ContextoUsuarioAuditoriaRdRecepcao?>(null);
    }

    ContextoUsuarioAuditoriaRdRecepcao? saida = Mapear(reivindicacoes);
    return Task.FromResult(saida);
  }

  private static ContextoUsuarioAuditoriaRdRecepcao? Mapear(ReivindicacoesJwtRecepcao r)
  {
    string cnpjEmpresa = SomenteDigitos(r.Cnpj);
    if (cnpjEmpresa.Length != 14)
    {
      return null;
    }

    string documentoUsuario = string.IsNullOrWhiteSpace(r.Cpf)
      ? SomenteDigitos(r.Documento)
      : SomenteDigitos(r.Cpf);

    if (documentoUsuario.Length is not (11 or 14))
    {
      return null;
    }

    string nomeEmpresa = Limitar((r.NomeEmpresa ?? string.Empty).Trim(), TamanhoMaximoNome);
    if (nomeEmpresa.Length == 0)
    {
      return null;
    }

    string nomeUsuario = Limitar(
      (r.NomeUsuario ?? r.NomeEmpresa ?? r.Documento).Trim(),
      TamanhoMaximoNome);

    if (nomeUsuario.Length == 0)
    {
      return null;
    }

    return new ContextoUsuarioAuditoriaRdRecepcao(
      Limitar(documentoUsuario, TamanhoMaximoCpfCnpj),
      nomeUsuario,
      Limitar(cnpjEmpresa, TamanhoMaximoCpfCnpj),
      nomeEmpresa);
  }

  private static string SomenteDigitos(string? valor)
  {
    if (string.IsNullOrWhiteSpace(valor))
    {
      return string.Empty;
    }

    return new string(valor.Where(char.IsDigit).ToArray());
  }

  private static string Limitar(string texto, int maximo)
  {
    if (texto.Length <= maximo)
    {
      return texto;
    }

    return texto[..maximo];
  }
}
