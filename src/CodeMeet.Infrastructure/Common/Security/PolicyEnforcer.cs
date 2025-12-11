using CodeMeet.Application.Common.Security;
using CodeMeet.Ddd.Application.Cqrs.Authorization;

using ErrorOr;

namespace CodeMeet.Infrastructure.Common.Security;

public class PolicyEnforcer : IPolicyEnforcer
{
    public ErrorOr<Success> Authorize<T>(
        IAuthorizeableRequest<T> request,
        CurrentUser currentUser,
        string policy)
    {
        return policy switch
        {
            Policy.SelfOrAdmin => SelfOrAdminPolicy(request, currentUser),
            Policy.AdminOnly => AdminOnlyPolicy(currentUser),
            _ => Error.Unauthorized(description: "Unknown policy name")
        };
    }

    private static ErrorOr<Success> SelfOrAdminPolicy<T>(IAuthorizeableRequest<T> request, CurrentUser currentUser) =>
        request.UserId == currentUser.Id || currentUser.Roles.Contains(Role.Admin)
            ? Result.Success
            : Error.Unauthorized(description: "Requesting user failed policy requirement");

    private static ErrorOr<Success> AdminOnlyPolicy(CurrentUser currentUser) =>
        currentUser.Roles.Contains(Role.Admin)
            ? Result.Success
            : Error.Unauthorized(description: "Admin role required");
}