using System.Linq.Expressions;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Domain;

namespace CodeMeet.Ddd.Infrastructure;

/// <summary>
/// Base interface for repositories. Repositories provide access to aggregates.
/// </summary>
/// <typeparam name="TAggregateRoot"></typeparam>
/// <typeparam name="TId"></typeparam>
public interface IRepository<TAggregateRoot, in TId>
    where TAggregateRoot : AggregationRoot<TId>
    where TId : notnull
{
    /// <summary>
    /// Gets an aggregate by its identifier. Throws if not found.
    /// </summary>
    Task<TAggregateRoot> GetAsync(TId id, CancellationToken token = default);

    /// <summary>
    /// Finds an aggregate by its identifier, returns null if not found.
    /// </summary>
    Task<TAggregateRoot?> FindAsync(TId id, CancellationToken token = default);

    /// <summary>
    /// Lists all aggregates.
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<IReadOnlyList<TAggregateRoot>> GetListAsync(CancellationToken token = default);

    /// <summary>
    /// Finds the first aggregate matching the predicate, returns null if not found.
    /// </summary>
    Task<TAggregateRoot?> FindAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default);

    /// <summary>
    /// Lists all aggregates matching the predicate.
    /// </summary>
    Task<IReadOnlyList<TAggregateRoot>> GetListAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default);

    /// <summary>
    /// Counts all aggregates matching the predicate.
    /// </summary>
    Task<int> CountAsync(Expression<Func<TAggregateRoot, bool>>? predicate = null, CancellationToken token = default);

    /// <summary>
    /// Checks if any aggregate matches the predicate.
    /// </summary>
    Task<bool> AnyAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default);

    /// <summary>
    /// Gets a queryable for advanced queries.
    /// </summary>
    IQueryable<TAggregateRoot> AsQueryable();

    /// <summary>
    /// Gets a paginated list of aggregates.
    /// </summary>
    Task<PaginationResult<TAggregateRoot>> GetPagedListAsync(IPaginationQuery pagination, CancellationToken token = default);

    /// <summary>
    /// Gets a paginated list of aggregates matching the predicate.
    /// </summary>
    Task<PaginationResult<TAggregateRoot>> GetPagedListAsync(Expression<Func<TAggregateRoot, bool>> predicate, IPaginationQuery pagination, CancellationToken token = default);

    /// <summary>
    /// Inserts a new aggregate.
    /// </summary>
    Task InsertAsync(TAggregateRoot entity, CancellationToken token = default);

    /// <summary>
    /// Updates an existing aggregate
    /// </summary>
    Task UpdateAsync(TAggregateRoot entity, CancellationToken token = default);

    /// <summary>
    /// Deletes an aggregate.
    /// </summary>
    Task DeleteAsync(TAggregateRoot entity, CancellationToken token = default);
}

/// <summary>
/// Repository for aggregates with Guid identifier.
/// </summary>
/// <typeparam name="TAggregateRoot">The aggregate root type.</typeparam>
public interface IRepository<TAggregateRoot> : IRepository<TAggregateRoot, Guid>
    where TAggregateRoot : AggregationRoot<Guid>;