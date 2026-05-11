using Suframa.RecepcaoRD.Application.Abstractions.AuditoriaRdRecepcao;
using Suframa.RecepcaoRD.Application.Abstractions.Persistence;
using Suframa.RecepcaoRD.Application.DTOs.AuditoriaRdRecepcao;
using Suframa.RecepcaoRD.Domain.Entities;

namespace Suframa.RecepcaoRD.Application.Features.RegistroAuditoriaRdRecepcao;

public sealed class RegistrarAuditoriaRdRecepcao : IRegistrarAuditoriaRdRecepcao
{
  private const int TamanhoMaximoCpfCnpj = 14;
  private const int TamanhoMaximoNome = 155;
  private const int TamanhoMaximoDescricao = 1000;

  private readonly IRepository<AuditoriaRdRecepcao> _auditorias;
  private readonly IRepository<ListaFuncionalidadeRecepcao> _funcionalidades;
  private readonly IUnitOfWork _unidadeDeTrabalho;
  private readonly IContextoUsuarioAuditoriaRdRecepcao _contextoSessao;

  public RegistrarAuditoriaRdRecepcao(
    IRepository<AuditoriaRdRecepcao> auditorias,
    IRepository<ListaFuncionalidadeRecepcao> funcionalidades,
    IUnitOfWork unidadeDeTrabalho,
    IContextoUsuarioAuditoriaRdRecepcao contextoSessao)
  {
    _auditorias = auditorias;
    _funcionalidades = funcionalidades;
    _unidadeDeTrabalho = unidadeDeTrabalho;
    _contextoSessao = contextoSessao;
  }

  public async Task RegistrarAsync(RegistrarAuditoriaRdRecepcaoComando comando, CancellationToken cancellationToken = default)
  {
    ContextoUsuarioAuditoriaRdRecepcao? contexto = await _contextoSessao.ObterAsync(cancellationToken);
    if (contexto is null)
    {
      throw new InvalidOperationException("Sessão inválida ou expirada. Autentique-se novamente.");
    }

    await RegistrarAsync(comando, contexto, cancellationToken);
  }

  public async Task RegistrarAsync(
    RegistrarAuditoriaRdRecepcaoComando comando,
    ContextoUsuarioAuditoriaRdRecepcao contextoUsuario,
    CancellationToken cancellationToken = default)
  {
    string? tipoNormalizado = NormalizarTipoAcao(comando.TipoAcao);
    if (tipoNormalizado is null)
    {
      throw new InvalidOperationException("Tipo de ação inválido. Utilize I, A ou E.");
    }

    bool funcionalidadeExiste =
      await _funcionalidades.AnyAsync(f => f.Id == comando.FuncionalidadeId, cancellationToken);
    if (!funcionalidadeExiste)
    {
      throw new InvalidOperationException("Funcionalidade informada não existe.");
    }

    string descricao = Limitar((comando.DescricaoAcao ?? string.Empty).Trim(), TamanhoMaximoDescricao);
    if (descricao.Length == 0)
    {
      throw new InvalidOperationException("Informe a descrição da ação.");
    }

    DateTime agoraUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc);
    AuditoriaRdRecepcao registro = AuditoriaRdRecepcao.Registrar(
      comando.IdReferencia,
      Limitar(contextoUsuario.CpfCnpjUsuarioSomenteDigitos, TamanhoMaximoCpfCnpj),
      Limitar(contextoUsuario.NomeUsuario, TamanhoMaximoNome),
      Limitar(contextoUsuario.CnpjEmpresaSomenteDigitos, TamanhoMaximoCpfCnpj),
      Limitar(contextoUsuario.NomeEmpresa, TamanhoMaximoNome),
      tipoNormalizado,
      agoraUtc,
      descricao,
      comando.FuncionalidadeId);

    await _auditorias.AddAsync(registro, cancellationToken);
    await _unidadeDeTrabalho.SaveChangesAsync(cancellationToken);
  }

  private static string? NormalizarTipoAcao(string? tipo)
  {
    if (string.IsNullOrWhiteSpace(tipo))
    {
      return null;
    }

    string t = tipo.Trim().ToUpperInvariant();
    return t is "I" or "A" or "E" ? t : null;
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
