using System.Reflection;
using CodeMeet.Ddd.Application.Cqrs;
using CodeMeet.Ddd.Application.Cqrs.Audit;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Application.Cqrs.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMeet.Ddd;

public static class CodeMeetDddModule
{
    /// <summary>
    /// Adds the CQRS dispatcher to the service collection.
    /// </summary>
    public static IServiceCollection AddDispatcher(this IServiceCollection services)
    {
        services.AddScoped<IDispatcher, Dispatcher>();
        services.AddScoped<IAuditContext, AuditContext>();
        return services;
    }

    /// <summary>
    /// Adds the CQRS dispatcher and registers all handlers from the specified assembly.
    /// </summary>
    public static IServiceCollection AddCqrs(this IServiceCollection services, Assembly assembly)
    {
        services.AddDispatcher();
        services.AddHandlersFromAssembly(assembly);
        return services;
    }

    /// <summary>
    /// Adds the CQRS dispatcher and registers all handlers from the specified assemblies.
    /// </summary>
    public static IServiceCollection AddCqrs(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddDispatcher();
        foreach (var assembly in assemblies)
        {
            services.AddHandlersFromAssembly(assembly);   
        }
        return services;
    }

    /// <summary>
    /// Registers all command and query handlers from the specified assembly.
    /// </summary>
    public static IServiceCollection AddHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && IsHandlerInterface(i.GetGenericTypeDefinition()))
                .Select(i => new { Implementation = t, Interface = i }));

        foreach (var handler in handlerTypes)
        {
            services.AddScoped(handler.Interface, handler.Implementation);
        }

        return services;
    }

    /// <summary>
    /// Registers all validators from the specified assembly.
    /// </summary>
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

    /// <summary>
    /// Registers a pipeline behavior.
    /// </summary>
    public static IServiceCollection AddPipelineBehavior<TBehavior>(this IServiceCollection services)
        where TBehavior : class
    {
        var behaviorType = typeof(TBehavior);
        var interfaces = behaviorType.GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehavior<,>));
        
        foreach (var @interface in interfaces)
        {
            services.AddScoped(@interface, behaviorType);
        }

        return services;
    }

    /// <summary>
    /// Registers a generic pipeline behavior that applies to all requests.
    /// </summary>
    public static IServiceCollection AddPipelineBehavior(this IServiceCollection services, Type behaviorType)
    {
        if (!behaviorType.IsGenericTypeDefinition)
            throw new ArgumentException("Behavior type must be an open generic type", nameof(behaviorType));

        services.AddScoped(typeof(IPipelineBehavior<,>), behaviorType);
        return services;
    }

    private static bool IsHandlerInterface(Type type)
    {
        return type == typeof(ICommandHandler<,>) || type == typeof(IQueryHandler<,>);
    }
}