using System.Collections.Concurrent;

using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users;

namespace CodeMeet.Infrastructure.Users;

public class InMemoryUserRepository : IRepository<User>
{
    private ConcurrentDictionary<Guid, User> _users = [];

    public async Task<User?> FindAsync(Guid id, CancellationToken token = default)
    {
        return !_users.TryGetValue(id, out var user) ? null : user;
    }

    public async Task<User> GetAsync(Guid id, CancellationToken token = default)
    {
        return _users.GetValueOrDefault(id) ?? throw new KeyNotFoundException(id.ToString());
    }

    public async Task<IReadOnlyList<User>> GetListAsync(CancellationToken token = default)
    {
        return _users.Values.ToList();
    }

    public Task InsertAsync(User entity, CancellationToken token = default)
    {
        if (!_users.TryAdd(entity.Id, entity))
        {
            throw new InvalidOperationException($"User with ID {entity.Id} already exists.");
        }
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User entity, CancellationToken token = default)
    {
        if (!_users.ContainsKey(entity.Id))
        {
            throw new KeyNotFoundException($"User with ID {entity.Id} not found.");
        }
        _users[entity.Id] = entity;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(User entity, CancellationToken token = default)
    {
        if (!_users.TryRemove(entity.Id, out _))
        {
            throw new KeyNotFoundException($"User with ID {entity.Id} not found.");
        }
        return Task.CompletedTask;
    }
}