namespace CodeMeet.Ddd.Application.Cqrs.Authorization;

public record CurrentUser(
    Guid Id,
    string Username,
    string Email,
    IReadOnlyList<string> Permissions,
    IReadOnlyList<string> Roles);