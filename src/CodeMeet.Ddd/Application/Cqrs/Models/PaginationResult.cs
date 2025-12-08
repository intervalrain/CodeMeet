namespace CodeMeet.Ddd.Application.Cqrs.Models;

/// <summary>
/// Represents a paginated result.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public record PaginationResult<T>(int Count, IReadOnlyList<T> Entities);
