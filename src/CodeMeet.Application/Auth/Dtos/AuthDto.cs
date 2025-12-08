namespace CodeMeet.Application.Auth.Dtos;

public record AuthDto(
    Guid UserId,
    string Username,
    string Token);