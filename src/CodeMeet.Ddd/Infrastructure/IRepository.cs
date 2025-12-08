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