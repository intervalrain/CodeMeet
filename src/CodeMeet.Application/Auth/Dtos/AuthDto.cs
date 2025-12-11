using CodeMeet.Application.Users.Dtos;

namespace CodeMeet.Application.Auth.Dtos;

public record AuthDto(
    UserDto User,
    string Token);