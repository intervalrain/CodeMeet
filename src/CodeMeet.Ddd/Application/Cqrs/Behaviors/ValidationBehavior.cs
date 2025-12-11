using CodeMeet.Ddd.Application.Cqrs.Behaviors.Options;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Application.Cqrs.Validation;
using Microsoft.Extensions.Options;

namespace CodeMeet.Ddd.Application.Cqrs.Behaviors;

/// <summary>
/// Pipeline behavior that validates requests before processing.
/// Collects all validation errors from registered validators and throws ValidationException if any fail.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResult">The type of result.</typeparam>
public sealed class ValidationBehavior<TRequest, TResult>(
    IEnumerable<IValidator<TRequest>> validators,
    IOptions<BehaviorOptions> options) : IPipelineBehavior<TRequest, TResult>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;
    private readonly ValidationBehaviorOptions _options = options.Value.Validation;

    public async Task<TResult> HandleAsync(TRequest request, Func<Task<TResult>> next, CancellationToken ct = default)
    {
        if (!_options.Enabled || !_options.Scope.Matches<TRequest>())
        {
            return await next();
        }

        var validators = _validators.ToList();

        if (validators.Count == 0)
            return await next();

        var validationTasks = validators.Select(v => v.ValidateAsync(request, ct));
        var validationResults = await Task.WhenAll(validationTasks);

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .ToList();

        if (errors.Count > 0)
            throw new ValidationException(errors);

        return await next();
    }
}
