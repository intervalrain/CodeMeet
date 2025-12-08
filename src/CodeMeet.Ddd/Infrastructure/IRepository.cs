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
    Task<AggregationRoot> GetAsync(TId id, CancellationToken token = default);
    Task<AggregationRoot?> FindAsync(TId id, CancellationToken token = default);
    Task<IReadOnlyList<TAggregateRoot>> GetListAsync(CancellationToken token = default);
    Task InsertAsync(AggregationRoot entity, CancellationToken token = default);
    Task DeleteAsync(AggregationRoot entity, CancellationToken token = default);
}

/// <summary>
/// Repository for aggregates with Guid identifier.
/// </summary>
/// <typeparam name="TAggregateRoot">The aggregate root type.</typeparam>
public interface IRepository<TAggregateRoot> : IRepository<AggregationRoot, Guid>
    where TAggregateRoot : AggregationRoot<Guid>;