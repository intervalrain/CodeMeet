using CodeMeet.Application.Common.Security;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeMeet.Infrastructure.Common.Persistence.Seeders;

/// <summary>
/// Seeds the default admin user if no admin exists.
/// </summary>
public class AdminUserSeeder(
    IRepository<User> userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    IOptions<AdminSeederOptions> options,
    ILogger<AdminUserSeeder> logger) : IDataSeeder
{
    public int Order => 0;

    private readonly AdminSeederOptions _options = options.Value;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Password))
        {
            logger.LogWarning("AdminSeeder:Password is not configured, skipping admin user seed");
            return;
        }

        var existingAdmin = await userRepository.FindAsync(
            u => u.Roles.Contains(Role.Admin),
            cancellationToken);

        if (existingAdmin is not null)
        {
            logger.LogDebug("Admin user already exists, skipping seed");
            return;
        }

        var adminUser = User.Create(
            _options.Username,
            passwordHasher.Hash(_options.Password),
            _options.Email,
            "Administrator");

        adminUser.Roles.Add(Role.Admin);

        await userRepository.InsertAsync(adminUser, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created default admin user: {Username}", _options.Username);
    }
}