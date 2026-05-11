namespace Suframa.RecepcaoRD.Application.DTOs.RelatorioDemonstrativo;

public sealed class AtualizarRdRecepcaoDto
{
  public int AnoBase { get; set; }

  public long PlanoId { get; set; }

  public string? RepresentanteLegal { get; set; }

  public string? Telefone { get; set; }

  public string? Email { get; set; }
}
