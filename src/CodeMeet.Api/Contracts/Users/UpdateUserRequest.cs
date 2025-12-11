namespace CodeMeet.Api.Contracts.Users;

public record UpdateUserRequest(
    string Password,
    string? NewPassword,
    string? DisplayName);