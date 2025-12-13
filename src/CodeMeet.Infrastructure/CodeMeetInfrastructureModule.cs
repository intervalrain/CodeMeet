using CodeMeet.Application.Common.Security;
using CodeMeet.Application.Gamification;
using CodeMeet.Application.Matches;
using CodeMeet.Ddd.Application.Cqrs.Authorization;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Infrastructure.Common.Persistence;
using CodeMeet.Infrastructure.Common.Persistence.InMemory;
using CodeMeet.Infrastructure.Common.Persistence.JsonFile;
using CodeMeet.Infrastructure.Common.Persistence.Seeders;
using CodeMeet.Infrastructure.Common.Security;
using CodeMeet.Infrastructure.Gamification;
using CodeMeet.Infrastructure.Matches;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CodeMeet.Infrastructure;

public static class CodeMeetInfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        // security
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IRefreshTokenGenerator, RefreshTokenGenerator>();
        services.AddSingleton<IPolicyEnforcer, PolicyEnforcer>();
        services.AddScoped<ICurrentUserProvider, CurrentUserProvider>();
        services.AddScoped<IAuthorizationService, AuthorizationService>();

        services.AddOptions<JwtSettings>()
            .Bind(configuration.GetSection(JwtSettings.Section))
            .Validate(s => !string.IsNullOrWhiteSpace(s.Secret))
            .ValidateOnStart();

        // persistences
        services.AddPersistence(configuration);

        // seeders
        services.Configure<AdminSeederOptions>(configuration.GetSection(AdminSeederOptions.SectionName));
        services.AddScoped<IDataSeeder, AdminUserSeeder>();

        // gamification
        services.AddScoped<IGamificationService, GamificationService>();

        // matching
        services.AddSingleton<IMatchQueueService, InMemoryMatchQueueService>();
        services.AddScoped<IMatchNotificationService, NoOpMatchNotificationService>();
        services.AddHostedService<MatchingBackgroundService>();

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PersistenceOptions>(configuration.GetSection(PersistenceOptions.SectionName));

        var options = configuration.GetSection(PersistenceOptions.SectionName).Get<PersistenceOptions>()
            ?? new PersistenceOptions();

        switch (options.Provider)
        {
            case PersistenceProvider.JsonFile:
                services.AddSingleton<JsonFileRepositoryRegistry>();
                services.AddSingleton(typeof(IRepository<>), typeof(JsonFileRepository<>));
                services.AddSingleton(typeof(IRepository<,>), typeof(JsonFileRepository<,>));
                services.AddScoped<IUnitOfWork, JsonFileUnitOfWork>();
                break;

            case PersistenceProvider.InMemory:
            default:
                services.AddSingleton(typeof(IRepository<>), typeof(InMemoryRepository<>));
                services.AddSingleton(typeof(IRepository<,>), typeof(InMemoryRepository<,>));
                services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();
                break;
        }

        return services;
    }
}