namespace Suframa.RecepcaoRD.Application.DTOs.RelatorioDemonstrativo;

public sealed class CriarRdRecepcaoDto
{
  public string? Cnpj { get; set; }

  public string? RazaoSocial { get; set; }

  public string? Endereco { get; set; }

  public string? InscricaoSuframa { get; set; }

  public int AnoBase { get; set; }

  public int TipoRd { get; set; }

  public long PlanoId { get; set; }

  public string? RepresentanteLegal { get; set; }

  public string? Telefone { get; set; }

  public string? Email { get; set; }

  public long? IdOrigem { get; set; }
}
