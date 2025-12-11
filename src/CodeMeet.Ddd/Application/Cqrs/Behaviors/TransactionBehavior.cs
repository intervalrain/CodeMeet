using CodeMeet.Ddd.Application.Cqrs.Behaviors.Options;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using Microsoft.Extensions.Options;

namespace CodeMeet.Ddd.Application.Cqrs.Behaviors;

/// <summary>
/// Pipeline behavior that wraps command execution in a transaction.
/// Only applies to commands (not queries) to maintain CQRS separation.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResult">The type of result.</typeparam>
public sealed class TransactionBehavior<TRequest, TResult>(IUnitOfWork unitOfWork, IOptions<BehaviorOptions> options) : IPipelineBehavior<TRequest, TResult>
    where TRequest : notnull
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly TransactionBehaviorOptions _options = options.Value.Transaction;

    public async Task<TResult> HandleAsync(TRequest request, Func<Task<TResult>> next, CancellationToken ct = default)
    {
        if (!_options.Enabled || !_options.Scope.Matches<TRequest>())
        {
            return await next();
        }

        var result = await next();

        await _unitOfWork.SaveChangesAsync(ct);

        return result;
    }
}
