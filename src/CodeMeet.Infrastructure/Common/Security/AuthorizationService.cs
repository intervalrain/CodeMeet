using CodeMeet.Ddd.Application.Cqrs.Authorization;
using ErrorOr;

namespace CodeMeet.Infrastructure.Common.Security;

public class AuthorizationService(
    IPolicyEnforcer policyEnforcer,
    ICurrentUserProvider currentUserProvider) : IAuthorizationService
{
    private readonly IPolicyEnforcer _policyEnforcer = policyEnforcer;
    private readonly ICurrentUserProvider _currentUserProvider = currentUserProvider;

    public ErrorOr<Success> Authorize<TRequest>(
        IAuthorizeableRequest<TRequest> request, 
        IReadOnlyList<string> requiredPermissions, 
        IReadOnlyList<string> requiredRoles, 
        IReadOnlyList<string> requiredPolicies)
    {
        var currentUser = _currentUserProvider.CurrentUser;

        if (requiredPermissions.Except(currentUser.Permissions).Any())
        {
            return Error.Unauthorized(description: "User is missing required permissions for taking this action.");
        }

        if (requiredRoles.Except(currentUser.Roles).Any())
        {
            return Error.Unauthorized(description: "User is missing required roles for taking this action.");
        }

        foreach (var policy in requiredPolicies)
        {
            var authorizationAgainstPolicyResult = _policyEnforcer.Authorize(request, currentUser, policy);

            if (authorizationAgainstPolicyResult.IsError)
            {
                return authorizationAgainstPolicyResult.Errors;
            }
        }

        return Result.Success;
    }
}