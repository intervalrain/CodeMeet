using CodeMeet.Application.Common.Security;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Infrastructure.Common.Persistences;
using CodeMeet.Infrastructure.Common.Security;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMeet.Infrastructure;

public static class CodeMeetInfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // security
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Section));

        // persistences
        services.AddSingleton(typeof(IRepository<>), typeof(InMemoryRepository<>));
        services.AddSingleton(typeof(IRepository<,>), typeof(InMemoryRepository<,>));
        services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
        
        return services;
    }
}