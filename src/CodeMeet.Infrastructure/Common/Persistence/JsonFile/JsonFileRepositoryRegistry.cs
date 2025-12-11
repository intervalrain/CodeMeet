namespace CodeMeet.Infrastructure.Common.Persistence.JsonFile;

/// <summary>
/// Registry for tracking all JSON file repositories.
/// </summary>
public class JsonFileRepositoryRegistry
{
    private readonly List<IJsonFilePersistable> _repositories = [];
    private readonly Lock _lock = new();

    public void Register(IJsonFilePersistable repository)
    {
        lock (_lock)
        {
            _repositories.Add(repository);
        }
    }

    public IReadOnlyList<IJsonFilePersistable> GetAll()
    {
        lock (_lock)
        {
            return _repositories.ToList();
        }
    }
}