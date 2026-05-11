using Microsoft.EntityFrameworkCore;

namespace Suframa.RecepcaoRD.Infrastructure.Persistence;

public sealed class CadsufDbContext : DbContext
{
  public CadsufDbContext(DbContextOptions<CadsufDbContext> options) : base(options) { }
}

