namespace CodeMeet.Ddd.Application.Cqrs.Validation;

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationException(IEnumerable<ValidationError> errors) : Exception("One or more validation errors occurred.")
{
    public IReadOnlyList<ValidationError> Errors { get; } = errors.ToList();

    public ValidationException(params ValidationError[] errors)
        : this((IEnumerable<ValidationError>)errors)
    {
    }
}