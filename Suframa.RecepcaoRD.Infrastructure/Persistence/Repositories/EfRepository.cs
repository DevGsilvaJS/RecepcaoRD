using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Suframa.RecepcaoRD.Application.Abstractions.Persistence;

namespace Suframa.RecepcaoRD.Infrastructure.Persistence.Repositories;

public sealed class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
  private readonly DbSet<TEntity> _set;

  public EfRepository(RecepcaoRDDbContext dbContext)
  {
    _set = dbContext.Set<TEntity>();
  }

  public async Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
  {
    return await _set.FindAsync([id], cancellationToken);
  }

  public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
  {
    return await _set.AnyAsync(predicate, cancellationToken);
  }

  public async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default)
  {
    return await _set.ToListAsync(cancellationToken);
  }

  public async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
  {
    return await _set.Where(predicate).ToListAsync(cancellationToken);
  }

  public async Task<PagedItems<TEntity>> ListarPaginadoAsync(
    Expression<Func<TEntity, bool>>? predicate,
    PagedOptions pagedFilter,
    Func<IQueryable<TEntity>, IQueryable<TEntity>>? incluirRelacionamentos = null,
    CancellationToken cancellationToken = default)
  {
    int tamanho = Math.Clamp(pagedFilter.Size ?? 10, 1, 200);
    if (pagedFilter.RegLimited is int limite && limite > 0)
    {
      tamanho = Math.Min(tamanho, limite);
    }

    int pagina = Math.Max(1, pagedFilter.Page ?? 1);
    int skip = (pagina * tamanho) - tamanho;

    IQueryable<TEntity> consulta = _set.AsNoTracking();
    if (incluirRelacionamentos is not null)
    {
      consulta = incluirRelacionamentos(consulta);
    }

    if (predicate is not null)
    {
      consulta = consulta.Where(predicate);
    }

    int total = await consulta.CountAsync(cancellationToken);

    consulta = OrdenacaoFiltroPaginado.Aplicar(consulta, pagedFilter);

    List<TEntity> itens = await consulta
      .Skip(skip)
      .Take(tamanho)
      .ToListAsync(cancellationToken);

    return new PagedItems<TEntity>(itens, total);
  }

  public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
  {
    await _set.AddAsync(entity, cancellationToken);
  }

  public void Update(TEntity entity)
  {
    _set.Update(entity);
  }

  public void Remove(TEntity entity)
  {
    _set.Remove(entity);
  }
}
