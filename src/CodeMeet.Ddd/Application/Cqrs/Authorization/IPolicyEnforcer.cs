using ErrorOr;

namespace CodeMeet.Ddd.Application.Cqrs.Authorization;

public interface IPolicyEnforcer
{
    public ErrorOr<Success> Authorize<T>(
        IAuthorizeableRequest<T> request,
        CurrentUser currentUser,
        string policy);
}