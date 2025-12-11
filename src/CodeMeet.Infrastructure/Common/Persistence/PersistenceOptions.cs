namespace CodeMeet.Infrastructure.Common.Persistence;

/// <summary>
/// Configuration options for persistence layer.
/// </summary>
public class PersistenceOptions
{
    public const string SectionName = "Persistence";

    /// <summary>
    /// The persistence provider to use. Default: InMemory.
    /// </summary>
    public PersistenceProvider Provider { get; set; } = PersistenceProvider.InMemory;

    /// <summary>
    /// Base directory for JSON file storage. Default: ./data
    /// </summary>
    public string DataDirectory { get; set; } = "./data";

    /// <summary>
    /// Whether to pretty-print JSON files. Default: true (development friendly).
    /// </summary>
    public bool IndentedJson { get; set; } = true;
}

public enum PersistenceProvider
{
    /// <summary>
    /// In-memory only, data lost on restart.
    /// </summary>
    InMemory,

    /// <summary>
    /// JSON file storage with in-memory cache.
    /// </summary>
    JsonFile
}
