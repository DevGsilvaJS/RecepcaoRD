using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Suframa.RecepcaoRD.Application.Abstractions.Autenticacao;
using Suframa.RecepcaoRD.Application.Configuracao;

namespace Suframa.RecepcaoRD.Infrastructure.Autenticacao;

public sealed class ArmazenamentoRefreshTokenEmMemoria : IArmazenamentoRefreshToken
{
  private readonly ConcurrentDictionary<string, RegistroRefreshToken> _tokens = new();
  private readonly AutenticacaoAmbienteOpcoes _opcoes;

  public ArmazenamentoRefreshTokenEmMemoria(IOptions<AutenticacaoAmbienteOpcoes> opcoes)
  {
    _opcoes = opcoes.Value;
  }

  public RefreshTokenEmitido EmitirParaDocumento(string documentoNormalizado)
  {
    string token = GerarTokenSeguro();
    DateTimeOffset expira = DateTimeOffset.UtcNow.AddDays(Math.Max(1, _opcoes.ExpiracaoRefreshTokenEmDias));
    _tokens[token] = new RegistroRefreshToken(documentoNormalizado, expira);
    return new RefreshTokenEmitido(token, expira);
  }

  public bool TentarConsumirParaRenovacao(
    string refreshToken,
    [NotNullWhen(true)] out string? documentoNormalizado)
  {
    documentoNormalizado = null;
    if (!_tokens.TryGetValue(refreshToken, out RegistroRefreshToken? registro))
    {
      return false;
    }

    if (registro.ExpiraEmUtc < DateTimeOffset.UtcNow)
    {
      _tokens.TryRemove(refreshToken, out _);
      return false;
    }

    if (!_tokens.TryRemove(refreshToken, out RegistroRefreshToken? removido))
    {
      return false;
    }

    documentoNormalizado = removido.DocumentoNormalizado;
    return true;
  }

  public void Revogar(string refreshToken)
  {
    _tokens.TryRemove(refreshToken, out _);
  }

  private static string GerarTokenSeguro()
  {
    Span<byte> buffer = stackalloc byte[32];
    RandomNumberGenerator.Fill(buffer);
    return Convert.ToBase64String(buffer)
      .TrimEnd('=')
      .Replace("+", "-", StringComparison.Ordinal)
      .Replace("/", "_", StringComparison.Ordinal);
  }

  private sealed record RegistroRefreshToken(string DocumentoNormalizado, DateTimeOffset ExpiraEmUtc);
}
