namespace CodeMeet.Ddd.Application.Cqrs.Models;

/// <summary>
/// Represents pagination query parameters.
/// </summary>
public record PaginationQuery(int PageNumber = 1, int PageSize = 20) : IPaginationQuery;