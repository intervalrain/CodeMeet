using CodeMeet.Ddd.Infrastructure;

namespace CodeMeet.Infrastructure.Common.Persistence.InMemory;

public class InMemoryUnitOfWork : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // No-op for in-memory storage
        return Task.FromResult(0);
    }
}
