using CodeMeet.Ddd.Infrastructure;

namespace CodeMeet.Infrastructure.Common.Persistence.JsonFile;

/// <summary>
/// Unit of work for JSON file persistence.
/// Coordinates saving all dirty repositories to disk.
/// </summary>
public class JsonFileUnitOfWork(JsonFileRepositoryRegistry registry) : IUnitOfWork
{
   public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var count = 0;
        foreach (var repository in registry.GetAll())
        {
            if (repository.IsDirty)
            {
                await repository.PersistAsync(cancellationToken);
                count++;
            }
        }
        return count;
    }
}