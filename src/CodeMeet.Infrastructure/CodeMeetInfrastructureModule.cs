using CodeMeet.Application.Common.Security;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Infrastructure.Common.Persistences;
using CodeMeet.Infrastructure.Common.Security;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMeet.Infrastructure;

public static class CodeMeetInfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton(typeof(IRepository<>), typeof(InMemoryRepository<>));
        services.AddSingleton(typeof(IRepository<,>), typeof(InMemoryRepository<,>));
        services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();
        
        return services;
    }
}