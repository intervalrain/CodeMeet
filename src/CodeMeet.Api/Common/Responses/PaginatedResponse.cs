namespace CodeMeet.Api.Common.Responses;

/// <summary>
/// Represents a paginated response.
/// </summary>
/// <typeparam name="T">The type of items in the response.</typeparam>
public record PaginatedResponse<T>(
    IReadOnlyList<T> Items,
    PaginationInfo Pagination);

/// <summary>
/// Contains pagination metadata.
/// </summary>
public record PaginationInfo(
    int Page,
    int PageSize,
    int TotalItems,
    int TotalPages);
