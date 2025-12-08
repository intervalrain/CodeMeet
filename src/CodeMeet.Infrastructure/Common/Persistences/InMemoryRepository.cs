using System.Collections.Concurrent;
using System.Linq.Expressions;
using CodeMeet.Ddd.Application.Cqrs.Models;
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

    public Task<TAggregateRoot?> FindAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
    {
        var compiled = predicate.Compile();
        var entity = _entities.Values.FirstOrDefault(compiled);
        return Task.FromResult(entity);
    }

    public Task<IReadOnlyList<TAggregateRoot>> GetListAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
    {
        var compiled = predicate.Compile();
        IReadOnlyList<TAggregateRoot> entities = _entities.Values.Where(compiled).ToList();
        return Task.FromResult(entities);
    }

    public Task<int> CountAsync(Expression<Func<TAggregateRoot, bool>>? predicate = null, CancellationToken token = default)
    {
        var count = predicate == null
            ? _entities.Count
            : _entities.Values.Count(predicate.Compile());
        return Task.FromResult(count);
    }

    public Task<bool> AnyAsync(Expression<Func<TAggregateRoot, bool>> predicate, CancellationToken token = default)
    {
        var compiled = predicate.Compile();
        var exists = _entities.Values.Any(compiled);
        return Task.FromResult(exists);
    }

    public IQueryable<TAggregateRoot> AsQueryable()
    {
        return _entities.Values.AsQueryable();
    }

    public Task<PaginationResult<TAggregateRoot>> GetPagedListAsync(IPaginationQuery pagination, CancellationToken token = default)
    {
        var totalCount = _entities.Count;
        var entities = _entities.Values
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        return Task.FromResult(new PaginationResult<TAggregateRoot>(totalCount, entities));
    }

    public Task<PaginationResult<TAggregateRoot>> GetPagedListAsync(Expression<Func<TAggregateRoot, bool>> predicate, IPaginationQuery pagination, CancellationToken token = default)
    {
        var compiled = predicate.Compile();
        var filtered = _entities.Values.Where(compiled).ToList();
        var totalCount = filtered.Count;
        var entities = filtered
            .Skip(pagination.Skip)
            .Take(pagination.Take)
            .ToList();

        return Task.FromResult(new PaginationResult<TAggregateRoot>(totalCount, entities));
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