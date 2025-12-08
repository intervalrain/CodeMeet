using ErrorOr;

namespace CodeMeet.Ddd.Application.Cqrs.Authorization;

public interface IAuthorizationService
{
    ErrorOr<Success> Authorize<TRequest>(
        IAuthorizeableRequest<TRequest> request,
        IReadOnlyList<string> requiredPermissions,
        IReadOnlyList<string> requiredRoles,
        IReadOnlyList<string> requiredPolicies);
}