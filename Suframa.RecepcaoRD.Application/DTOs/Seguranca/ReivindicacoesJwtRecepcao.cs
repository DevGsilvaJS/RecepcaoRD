namespace Suframa.RecepcaoRD.Application.DTOs.Seguranca;

/// <summary>
/// Reivindicações esperadas no JWT de sessão (cookie ou portador).
/// </summary>
public sealed record ReivindicacoesJwtRecepcao(
  string Documento,
  string? Cpf,
  string? Cnpj,
  string? NomeEmpresa,
  string? NomeUsuario);
