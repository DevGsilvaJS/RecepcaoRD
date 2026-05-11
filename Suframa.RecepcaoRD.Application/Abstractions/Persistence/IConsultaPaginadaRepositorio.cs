using System.Linq.Expressions;

namespace Suframa.RecepcaoRD.Application.Abstractions.Persistence;

/// <summary>
/// Contrato reutilizável de listagem paginada para qualquer entidade persistida via EF.
/// </summary>
public interface IConsultaPaginadaRepositorio<TEntidade> where TEntidade : class
{
  /// <summary>
  /// Lista com predicado opcional e opções de paginação/ordenação (<see cref="PagedOptions"/>).
  /// </summary>
  Task<PagedItems<TEntidade>> ListarPaginadoAsync(
    Expression<Func<TEntidade, bool>>? predicate,
    PagedOptions pagedFilter,
    Func<IQueryable<TEntidade>, IQueryable<TEntidade>>? incluirRelacionamentos = null,
    CancellationToken cancellationToken = default);
}
