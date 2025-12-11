using System.Reflection;
using CodeMeet.Ddd;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMeet.Application;

public static class CodeMeetApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddCqrs(assembly);
        services.AddValidatorsFromAssembly(assembly);
        services.AddStandardBehaviors(configuration);

        return services;
    }
}