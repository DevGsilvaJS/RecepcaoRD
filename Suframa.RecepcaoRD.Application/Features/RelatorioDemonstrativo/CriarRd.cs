using Suframa.RecepcaoRD.Application.Abstractions.Persistence;
using Suframa.RecepcaoRD.Application.Abstractions.RelatorioDemonstrativo;
using Suframa.RecepcaoRD.Application.DTOs.RelatorioDemonstrativo;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Application.Features.RelatorioDemonstrativo;

public sealed class CriarRd : ICriarRd
{
  private readonly IRepository<RdRecepcao> _rds;
  private readonly IRepository<PlanoRecepcao> _planos;
  private readonly IUnitOfWork _unidadeDeTrabalho;

  public CriarRd(
    IRepository<RdRecepcao> rds,
    IRepository<PlanoRecepcao> planos,
    IUnitOfWork unidadeDeTrabalho)
  {
    _rds = rds;
    _planos = planos;
    _unidadeDeTrabalho = unidadeDeTrabalho;
  }

  public async Task CriarAsync(CriarRdRecepcaoDto requisicao, CancellationToken cancellationToken = default)
  {
    string cnpj = SomenteDigitos(requisicao.Cnpj);
    if (cnpj.Length != 14)
    {
      throw new InvalidOperationException("CNPJ da empresa inválido ou ausente.");
    }

    if (!Enum.IsDefined(typeof(TipoRdRecepcao), requisicao.TipoRd))
    {
      throw new InvalidOperationException("Tipo de RD inválido.");
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

    if (SomenteDigitos(plano.CnpjEmpresa) != cnpj)
    {
      throw new InvalidOperationException("Plano não pertence à empresa informada.");
    }

    long? idOrigem = requisicao.IdOrigem;
    if (idOrigem is long origemId)
    {
      RdRecepcao? origem = await _rds.GetByIdAsync(origemId, cancellationToken);
      if (origem is null || origem.Cnpj != cnpj)
      {
        throw new InvalidOperationException("RD de origem inválida ou de outra empresa.");
      }
    }

    string razao = Limitar((requisicao.RazaoSocial ?? string.Empty).Trim(), 155);
    string endereco = Limitar((requisicao.Endereco ?? string.Empty).Trim(), 255);
    string inscricao = Limitar((requisicao.InscricaoSuframa ?? string.Empty).Trim(), 14);
    string representante = Limitar((requisicao.RepresentanteLegal ?? string.Empty).Trim(), 100);
    string telefone = Limitar((requisicao.Telefone ?? string.Empty).Trim(), 20);
    string email = Limitar((requisicao.Email ?? string.Empty).Trim(), 255);

    if (razao.Length == 0 || endereco.Length == 0 || representante.Length == 0)
    {
      throw new InvalidOperationException("Preencha razão social, endereço e representante legal.");
    }

    if (telefone.Length == 0 || email.Length == 0)
    {
      throw new InvalidOperationException("Preencha telefone e e-mail.");
    }

    TipoRdRecepcao tipo = (TipoRdRecepcao)requisicao.TipoRd;
    RdRecepcao novo = RdRecepcao.Criar(
      cnpj,
      razao,
      endereco,
      inscricao,
      ano,
      tipo,
      requisicao.PlanoId,
      representante,
      telefone,
      email,
      idOrigem);

    await _rds.AddAsync(novo, cancellationToken);
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
