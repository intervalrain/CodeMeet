namespace CodeMeet.Api.Models.Users;

public record UpdateUserDto(
    string Password,
    string? NewPassword,
    string? DisplayName);