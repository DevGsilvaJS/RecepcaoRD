using Microsoft.AspNetCore.Mvc;
using Suframa.RecepcaoRD.Application.Abstractions.Persistence;
using Suframa.RecepcaoRD.Application.Abstractions.PlanosRecepcao;
using Suframa.RecepcaoRD.Application.Abstractions.RelatorioDemonstrativo;
using Suframa.RecepcaoRD.Application.Common;
using Suframa.RecepcaoRD.Application.DTOs.PlanosRecepcao;
using Suframa.RecepcaoRD.Application.DTOs.RelatorioDemonstrativo;

namespace Suframa.RecepcaoRD.API.Controllers;

[ApiController]
[Route("api/relatorio-demonstrativo")]
public sealed class RelatorioDemonstrativoController : ControllerBase
{
  private readonly IListarPlanosRecepcao _listarPlanos;
  private readonly IListarRd _listarRd;
  private readonly ICriarRd _criarRd;
  private readonly IAtualizarRd _atualizarRd;

  public RelatorioDemonstrativoController(
    IListarPlanosRecepcao listarPlanos,
    IListarRd listarRd,
    ICriarRd criarRd,
    IAtualizarRd atualizarRd)
  {
    _listarPlanos = listarPlanos;
    _listarRd = listarRd;
    _criarRd = criarRd;
    _atualizarRd = atualizarRd;
  }

  [HttpGet]
  public async Task<ActionResult<ApiResponse<PagedItems<RdRecepcaoListaItemDto>>>> ListarRd(
    [FromQuery] ListarRdConsulta consulta,
    CancellationToken cancellationToken = default)
  {
    PagedItems<RdRecepcaoListaItemDto> resultado =
      await _listarRd.ListarAsync(consulta, cancellationToken);

    return Ok(new ApiResponse<PagedItems<RdRecepcaoListaItemDto>>
    {
      Sucesso = true,
      Mensagem = null,
      Dados = resultado,
      Erros = null,
    });
  }

  [HttpPost]
  public async Task<ActionResult<ApiResponse<object?>>> CriarRd(
    [FromBody] CriarRdRecepcaoDto requisicao,
    CancellationToken cancellationToken = default)
  {
    try
    {
      await _criarRd.CriarAsync(requisicao, cancellationToken);
      return Ok(new ApiResponse<object?>
      {
        Sucesso = true,
        Mensagem = "Gravado com sucesso.",
        Dados = null,
        Erros = null,
      });
    }
    catch (InvalidOperationException excecao)
    {
      return Ok(new ApiResponse<object?>
      {
        Sucesso = false,
        Mensagem = excecao.Message,
        Dados = null,
        Erros = null,
      });
    }
  }

  [HttpPut("{id:long}")]
  public async Task<ActionResult<ApiResponse<object?>>> AtualizarRd(
    long id,
    [FromBody] AtualizarRdRecepcaoDto requisicao,
    CancellationToken cancellationToken = default)
  {
    try
    {
      await _atualizarRd.AtualizarAsync(id, requisicao, cancellationToken);
      return Ok(new ApiResponse<object?>
      {
        Sucesso = true,
        Mensagem = "Gravado com sucesso.",
        Dados = null,
        Erros = null,
      });
    }
    catch (KeyNotFoundException)
    {
      return Ok(new ApiResponse<object?>
      {
        Sucesso = false,
        Mensagem = "RD não encontrado.",
        Dados = null,
        Erros = null,
      });
    }
    catch (InvalidOperationException excecao)
    {
      return Ok(new ApiResponse<object?>
      {
        Sucesso = false,
        Mensagem = excecao.Message,
        Dados = null,
        Erros = null,
      });
    }
  }

  [HttpGet("planos")]
  public async Task<ActionResult<ApiResponse<IReadOnlyList<PlanoRecepcaoListaItemDto>>>> ListarPlanos(
    [FromQuery(Name = "cnpj")] string? cnpj,
    CancellationToken cancellationToken)
  {
    IReadOnlyList<PlanoRecepcaoListaItemDto> itens = await _listarPlanos.ExecutarAsync(cnpj, cancellationToken);
    return Ok(new ApiResponse<IReadOnlyList<PlanoRecepcaoListaItemDto>>
    {
      Sucesso = true,
      Mensagem = null,
      Dados = itens,
      Erros = null,
    });
  }
}
