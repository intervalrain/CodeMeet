using System.Reflection;
using CodeMeet.Ddd.Application.Cqrs.Authorization;
using CodeMeet.Ddd.Application.Cqrs.Models;
using ErrorOr;

namespace CodeMeet.Ddd.Application.Cqrs.Behaviors;

/// <summary>
/// Pipeline behavior that enforces authorization based on <see cref="AuthorizaAttribute"/> decorations.
/// Only applies to requests that implement <see cref="IAuthorizeableRequest{TResponse}"/> 
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResponse">The type of response (must implement IErrorOr).</typeparam>
public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuthorizeableRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IAuthorizationService _authorizationService;

    public AuthorizationBehavior(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task<TResponse> HandleAsync(
        TRequest request, 
        Func<Task<TResponse>> next, 
        CancellationToken token = default)
    {
        var authorizeAttributes = request.GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToList();

        if (!authorizeAttributes.Any())
        {
            return await next();   
        }

        var requiredPermissions = authorizeAttributes
            .SelectMany(attr => attr.Permissions?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [])
            .Select(p => p.Trim())
            .Distinct()
            .ToList();
        
        var requiredRoles = authorizeAttributes
            .SelectMany(attr => attr.Roles?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [])
            .Select(r => r.Trim())
            .Distinct()
            .ToList();
        
        var requiredPolicies = authorizeAttributes
            .SelectMany(attr => attr.Policies?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? [])
            .Select(p => p.Trim())
            .Distinct()
            .ToList();

        var authorizationResult = _authorizationService.Authorize(
            request,
            requiredRoles,
            requiredPermissions,
            requiredPolicies);

        if (authorizationResult.IsError)
        {
            return (dynamic)authorizationResult.Errors;
        }
        return await next();
    }
}
