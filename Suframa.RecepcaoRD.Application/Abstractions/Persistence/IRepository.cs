using System.Linq;
using System.Linq.Expressions;

namespace Suframa.RecepcaoRD.Application.Abstractions.Persistence;

public interface IRepository<TEntity> : IConsultaPaginadaRepositorio<TEntity> where TEntity : class
{
  Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

  Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

  Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default);

  Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

  Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

  void Update(TEntity entity);

  void Remove(TEntity entity);
}

