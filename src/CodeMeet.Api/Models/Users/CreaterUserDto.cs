using System.ComponentModel.DataAnnotations;

namespace CodeMeet.Api.Models.Users;

public record CreateUserDto(
    string Username,
    string Password,
    [EmailAddress] string Email,
    string? DisplayName
);