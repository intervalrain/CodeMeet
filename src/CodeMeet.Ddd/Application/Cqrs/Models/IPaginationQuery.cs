namespace CodeMeet.Ddd.Application.Cqrs.Models;

/// <summary>
/// Pagination parameters without result type binding.
/// Used by repositories and infrastructure layer.
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

/// <summary>
/// Pagination query with result type binding.
/// Used by application layer queries.
/// </summary>
public interface IPaginationQuery<TResult> : IPaginationQuery, IQuery<TResult>
{
}
