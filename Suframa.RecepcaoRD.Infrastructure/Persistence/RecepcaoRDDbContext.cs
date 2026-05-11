using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Suframa.RecepcaoRD.Application.Abstractions.Persistence;

namespace Suframa.RecepcaoRD.Infrastructure.Persistence;

public sealed class RecepcaoRDDbContext : DbContext, IUnitOfWork
{
  private IDbContextTransaction? _transaction;

  public RecepcaoRDDbContext(DbContextOptions<RecepcaoRDDbContext> options) : base(options) { }

  public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
  {
    return await base.SaveChangesAsync(cancellationToken);
  }

  public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
  {
    if (_transaction is not null)
    {
      throw new InvalidOperationException("Já existe uma transação ativa.");
    }

    _transaction = await Database.BeginTransactionAsync(cancellationToken);
  }

  public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
  {
    if (_transaction is null)
    {
      return;
    }

    await _transaction.CommitAsync(cancellationToken);
    await _transaction.DisposeAsync();
    _transaction = null;
  }

  public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
  {
    if (_transaction is null)
    {
      return;
    }

    await _transaction.RollbackAsync(cancellationToken);
    await _transaction.DisposeAsync();
    _transaction = null;
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(RecepcaoRDDbContext).Assembly);
    base.OnModelCreating(modelBuilder);
  }
}
