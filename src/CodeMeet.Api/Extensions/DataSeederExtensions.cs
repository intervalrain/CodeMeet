using CodeMeet.Infrastructure.Common.Persistence;

namespace CodeMeet.Api.Extensions;

public static class DataSeederExtensions
{
    /// <summary>
    /// Runs all registered data seeders.
    /// </summary>
    public static async Task SeedDataAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();
        var seeders = scope.ServiceProvider.GetServices<IDataSeeder>()
            .OrderBy(s => s.Order);

        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync(cancellationToken);
        }
    }
}
