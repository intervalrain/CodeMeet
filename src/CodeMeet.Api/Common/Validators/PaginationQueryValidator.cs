
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Application.Cqrs.Validation;

namespace CodeMeet.Api.Common.Validators;

public class PaginationQueryValidator : IValidator<IPaginationQuery>
{
    public Task<ValidationResult> ValidateAsync(IPaginationQuery instance, CancellationToken ct = default)
    {
        var errors = new List<ValidationError>();

        if (instance.PageNumber < 1)
            errors.Add(new ValidationError(nameof(IPaginationQuery.PageNumber), "Page number must be greater than or equal to 1"));

        if (instance.PageSize < 1 || instance.PageSize > 100)
            errors.Add(new ValidationError(nameof(IPaginationQuery.PageSize), "Page size must be between 1 and 100"));

        return Task.FromResult(errors.Count > 0
            ? ValidationResult.Failure(errors)
            : ValidationResult.Success());
    }
}