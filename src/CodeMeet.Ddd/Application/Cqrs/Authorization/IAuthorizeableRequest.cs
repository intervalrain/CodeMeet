using CodeMeet.Ddd.Application.Cqrs.Models;

namespace CodeMeet.Ddd.Application.Cqrs.Authorization;

/// <summary>
/// Marker interface for requests that require authorization.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IAuthorizeableRequest<TResponse>
{
    Guid UserId { get; }
};

/// <summary>
/// Command that requires authorization checks.
/// Use this instead of ICommand when you want to apply [Authorize] attributes.
/// </summary>
/// <typeparam name="TResult">The result type.</typeparam>
public interface IAuthorizeableCommand<TResult> : ICommand<TResult>, IAuthorizeableRequest<TResult>;

/// <summary>
/// Command without result that requires authorization checks.
/// </summary>
public interface IAuthorizeableCommand : IAuthorizeableCommand<Unit>;

/// <summary>
/// Query that requires authorization checks.
/// Use this instead of IQuery when you want to apply [Authorize] attributes.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IAuthorizeableQuery<TResponse> : IQuery<TResponse>, IAuthorizeableRequest<TResponse>;