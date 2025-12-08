using System.Collections.Concurrent;

using CodeMeet.Ddd.Domain;
using CodeMeet.Ddd.Infrastructure;

namespace CodeMeet.Infrastructure.Common.Persistences;

public class InMemoryRepository<TAggregateRoot, TId> : IRepository<TAggregateRoot, TId>
    where TAggregateRoot : AggregationRoot<TId>
    where TId : notnull
{
    private readonly ConcurrentDictionary<TId, TAggregateRoot> _entities = [];

    public Task<TAggregateRoot?> FindAsync(TId id, CancellationToken token = default)
    {
        _entities.TryGetValue(id, out var entity);
        return Task.FromResult(entity);
    }

    public Task<TAggregateRoot> GetAsync(TId id, CancellationToken token = default)
    {
        if (!_entities.TryGetValue(id, out var entity))
            throw new KeyNotFoundException($"Entity with id {id} not found");
        
        return Task.FromResult(entity);
    }

    public Task<IReadOnlyList<TAggregateRoot>> GetListAsync(CancellationToken token = default)
    {
        IReadOnlyList<TAggregateRoot> entities = _entities.Values.ToList();
        return Task.FromResult(entities);
    }

    public Task InsertAsync(TAggregateRoot entity, CancellationToken token = default)
    {
        _entities.TryAdd(entity.Id, entity);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TAggregateRoot entity, CancellationToken token = default)
    {
        _entities[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(TAggregateRoot entity, CancellationToken token = default)
    {
        _entities.TryRemove(entity.Id, out _);
        return Task.CompletedTask;
    }
}

public class InMemoryRepository<TAggregateRoot> : InMemoryRepository<TAggregateRoot, Guid>, IRepository<TAggregateRoot>
    where TAggregateRoot : AggregationRoot<Guid>;