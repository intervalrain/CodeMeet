namespace CodeMeet.Infrastructure.Common.Persistence.JsonFile;


/// <summary>
/// Interface for JSON file repositories that can be persisted.
/// </summary>
public interface IJsonFilePersistable
{
    bool IsDirty { get; }
    Task PersistAsync(CancellationToken cancellationToken = default);
}