using System.Collections;
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
    IServiceProvider serviceProvider,
    IOptions<BehaviorOptions> options) : IPipelineBehavior<TRequest, TResult>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly ValidationBehaviorOptions _options = options.Value.Validation;

    public async Task<TResult> HandleAsync(TRequest request, Func<Task<TResult>> next, CancellationToken ct = default)
    {
        if (!_options.Enabled || !_options.Scope.Matches<TRequest>())
        {
            return await next();
        }

        var allValidators = GetAllValidators();

        if (allValidators.Count == 0)
            return await next();

        var validationTasks = allValidators.Select(v => v(request, ct));
        var validationResults = await Task.WhenAll(validationTasks);

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .ToList();

        if (errors.Count > 0)
            throw new ValidationException(errors);

        return await next();
    }

    private List<Func<TRequest, CancellationToken, Task<ValidationResult>>> GetAllValidators()
    {
        var validatorFuncs = new List<Func<TRequest, CancellationToken, Task<ValidationResult>>>();

        // Add direct validators for TRequest
        foreach (var validator in _validators)
        {
            validatorFuncs.Add(validator.ValidateAsync);
        }

        // Add validators for interfaces that TRequest implements
        var interfaces = typeof(TRequest).GetInterfaces();
        foreach (var iface in interfaces)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(iface);
            var interfaceValidators = _serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(validatorType));

            if (interfaceValidators is IEnumerable enumerable)
            {
                foreach (var validator in enumerable)
                {
                    var validateMethod = validatorType.GetMethod("ValidateAsync");
                    if (validateMethod != null)
                    {
                        validatorFuncs.Add((req, ct) =>
                        {
                            var task = validateMethod.Invoke(validator, [req, ct]);
                            return (Task<ValidationResult>)task!;
                        });
                    }
                }
            }
        }

        return validatorFuncs;
    }
}
