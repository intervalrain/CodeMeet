using System.Reflection;
using CodeMeet.Ddd.Application.Cqrs;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Application.Cqrs.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMeet.Ddd;

public static class CodeMeetDddModule
{
    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.AddScoped<IDispatcher, Dispatcher>();
        return services;
    }

    public static IServiceCollection AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        // Register Command Handlers
        var commandHandlerTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
                .Select(i => new { Implementation = t, Interface = i }));

        foreach (var handler in commandHandlerTypes)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }

        // Register Query Handlers
        var queryHandlerTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
                .Select(i => new { Implementation = t, Interface = i }));

        foreach (var handler in queryHandlerTypes)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }

        return services;
    }

    public static IServiceCollection AddValidatorsFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var validatorTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>))
                .Select(i => new { Implementation = t, Interface = i }));

        foreach (var validator in validatorTypes)
        {
            services.AddScoped(validator.Interface, validator.Implementation);
        }

        return services;
    }
}