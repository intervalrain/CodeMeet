namespace CodeMeet.Infrastructure.Common.Persistence;

/// <summary>
/// Interface for data seeders that initialize default data.
/// </summary>
public interface IDataSeeder
{
    /// <summary>
    /// Order of execution. Lower numbers run first.
    /// </summary>
    int Order => 0;

    /// <summary>
    /// Seeds the data.
    /// </summary>
    Task SeedAsync(CancellationToken cancellationToken = default);
}