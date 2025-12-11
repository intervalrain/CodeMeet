namespace CodeMeet.Ddd.Application.Cqrs.Models;

/// <summary>
/// Represents a paginated result.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public record PaginationResult<T>(
    int Page,
    int PageSize,
    int TotalCount,
    IReadOnlyList<T> Items)
{
    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
}
