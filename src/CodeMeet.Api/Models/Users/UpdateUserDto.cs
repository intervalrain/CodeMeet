namespace CodeMeet.Api.Models.Users;

public record UpdateUserDto(
    Guid Id,
    string Password,
    string NewPassword
);