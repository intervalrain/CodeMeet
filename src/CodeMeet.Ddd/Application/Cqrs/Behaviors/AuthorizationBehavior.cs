using System.Reflection;
using CodeMeet.Ddd.Application.Cqrs.Authorization;
using CodeMeet.Ddd.Application.Cqrs.Behaviors.Options;
using CodeMeet.Ddd.Application.Cqrs.Models;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace CodeMeet.Ddd.Application.Cqrs.Behaviors;

/// <summary>
/// Pipeline behavior that enforces authorization based on <see cref="AuthorizeAttribute"/> decorations.
/// Only applies to requests that implement <see cref="IAuthorizeableRequest{TResponse}"/>
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResponse">The type of response (must implement IErrorOr).</typeparam>
public class AuthorizationBehavior<TRequest, TResponse>(
    IAuthorizationService authorizationService,
    IOptions<BehaviorOptions> options) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IAuthorizeableRequest<TResponse>
    where TResponse : IErrorOr
{
    private readonly IAuthorizationService _authorizationService = authorizationService;
    private readonly AuthorizationBehaviorOptions _options = options.Value.Authorization;

    public async Task<TResponse> HandleAsync(
        TRequest request,
        Func<Task<TResponse>> next,
        CancellationToken token = default)
    {
        if (!_options.Enabled)
        {
            return await next();
        }

        var authorizeAttributes = request.GetType()
            .GetCustomAttributes<AuthorizeAttribute>()
            .ToList();

        if (authorizeAttributes.Count == 0)
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
            requiredPermissions,
            requiredRoles,
            requiredPolicies);

        if (authorizationResult.IsError)
        {
            return (dynamic)authorizationResult.Errors;
        }
        return await next();
    }
}
