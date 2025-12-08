namespace CodeMeet.Api.Models.Auth;

public record LoginDto(
    string Username,
    string Password
);