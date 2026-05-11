using Suframa.RecepcaoRD.Application.Abstractions.Persistence;
using Suframa.RecepcaoRD.Application.Abstractions.RelatorioDemonstrativo;
using Suframa.RecepcaoRD.Application.DTOs.RelatorioDemonstrativo;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Application.Features.RelatorioDemonstrativo;

public sealed class AtualizarRd : IAtualizarRd
{
  private readonly IRepository<RdRecepcao> _rds;
  private readonly IRepository<PlanoRecepcao> _planos;
  private readonly IUnitOfWork _unidadeDeTrabalho;

  public AtualizarRd(
    IRepository<RdRecepcao> rds,
    IRepository<PlanoRecepcao> planos,
    IUnitOfWork unidadeDeTrabalho)
  {
    _rds = rds;
    _planos = planos;
    _unidadeDeTrabalho = unidadeDeTrabalho;
  }

  public async Task AtualizarAsync(
    long idRd,
    AtualizarRdRecepcaoDto requisicao,
    CancellationToken cancellationToken = default)
  {
    RdRecepcao? rd = await _rds.GetByIdAsync(idRd, cancellationToken);
    if (rd is null)
    {
      throw new KeyNotFoundException();
    }

    int ano = requisicao.AnoBase;
    if (ano < 1900 || ano > 2100)
    {
      throw new InvalidOperationException("Ano base inválido.");
    }

    PlanoRecepcao? plano = await _planos.GetByIdAsync(requisicao.PlanoId, cancellationToken);
    if (plano is null)
    {
      throw new InvalidOperationException("Plano não encontrado.");
    }

    if (SomenteDigitos(plano.CnpjEmpresa) != SomenteDigitos(rd.Cnpj))
    {
      throw new InvalidOperationException("Plano não pertence à empresa do RD.");
    }

    string representante = Limitar((requisicao.RepresentanteLegal ?? string.Empty).Trim(), 100);
    string telefone = Limitar((requisicao.Telefone ?? string.Empty).Trim(), 20);
    string email = Limitar((requisicao.Email ?? string.Empty).Trim(), 255);

    if (representante.Length == 0 || telefone.Length == 0 || email.Length == 0)
    {
      throw new InvalidOperationException("Preencha representante legal, telefone e e-mail.");
    }

    rd.AtualizarCadastro(ano, requisicao.PlanoId, representante, telefone, email);
    await _unidadeDeTrabalho.SaveChangesAsync(cancellationToken);
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
