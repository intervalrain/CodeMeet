namespace CodeMeet.Ddd.Application.Cqrs.Models;

/// <summary>
/// Pagination query parameters.
/// </summary>
public interface IPaginationQuery
{
    /// <summary>
    /// Page number (1-based).
    /// </summary>
    int PageNumber { get; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Number of items to skip.
    /// </summary>
    int Skip => (PageNumber - 1) * PageSize;

    /// <summary>
    /// Number of items to take.
    /// </summary>
    int Take => PageSize;
}
