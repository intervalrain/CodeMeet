using CodeMeet.Application.Common.Security;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users;
using CodeMeet.Infrastructure.Common.Security;
using CodeMeet.Infrastructure.Users;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMeet.Infrastructure;

public static class CodeMeetInfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IRepository<User>, InMemoryUserRepository>();
        return services;
    }
}