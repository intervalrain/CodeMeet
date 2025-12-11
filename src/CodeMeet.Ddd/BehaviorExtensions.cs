using CodeMeet.Ddd.Application.Cqrs.Behaviors;
using CodeMeet.Ddd.Application.Cqrs.Behaviors.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CodeMeet.Ddd;

public static class BehaviorExtensions
{
    /// <summary>
    /// Adds all standard pipeline behaviors in the recommended order:
    /// 1. Audit (trace ID setup)
    /// 2. Logging
    /// 3. Authorization (AOP-style, for requests with [Authorize] attribute)
    /// 4. Validation
    /// 5. Transaction
    /// </summary>
    /// <remarks>
    /// Note: Authorization behavior requires <see cref="IRequestAuthorizationService"/>
    /// to be registered separately (typically in the Application module).
    /// </remarks>
    public static IServiceCollection AddStandardBehaviors(this IServiceCollection services, IConfiguration? configuration = null)
    {
        if (configuration is not null)
        {
            services.Configure<BehaviorOptions>(configuration.GetSection(BehaviorOptions.SectionName));
        }
        else
        {
            services.Configure<BehaviorOptions>(_ => { });
        }

        services.AddAuditBehavior();
        services.AddLoggingBehavior();
        services.AddAuthorizationBehavior();
        services.AddValidationBehavior();
        services.AddTransactionBehavior();

        return services;
    }

    /// <summary>
    /// Adds the logging pipeline behavior.
    /// </summary>
    public static IServiceCollection AddLoggingBehavior(this IServiceCollection services)
    {
        services.AddPipelineBehavior(typeof(LoggingBehavior<,>));
        return services;
    }

    /// <summary>
    /// Adds the validation pipeline behavior.
    /// Requires validators to be registered via AddValidatorsFromAssembly.
    /// </summary>
    public static IServiceCollection AddValidationBehavior(this IServiceCollection services)
    {
        services.AddPipelineBehavior(typeof(ValidationBehavior<,>));
        return services; 
    }

    /// <summary>
    /// Adds the transaction pipeline behavior.
    /// Requires IUnitOfWork to be registered.
    public static IServiceCollection AddTransactionBehavior(this IServiceCollection services)
    {
        services.AddPipelineBehavior(typeof(TransactionBehavior<,>));
        return services; 
    }

    /// <summary>
    /// Adds the authorization pipeline behavior for AOP-style authorization.
    /// Requires IAuthorizationService to be registered.
    /// </summary>
    /// <remarks>
    /// This behavior only applies to requests that implement
    /// <see cref="IAuthorizeableRequest{TResponse}"/> and are decorated with
    /// <see cref="AuthorizeAttribute"/>
    /// </remarks>
    public static IServiceCollection AddAuthorizationBehavior(this IServiceCollection services)
    {
        services.AddPipelineBehavior(typeof(AuthorizationBehavior<,>));
        return services; 
    }

    /// <summary>
    /// Adds the audit pipeline behavior with trace ID and correlation ID support.
    /// </summary>
    public static IServiceCollection AddAuditBehavior(this IServiceCollection services)
    {
        services.AddPipelineBehavior(typeof(AuditBehavior<,>));
        return services; 
    }
}