using System.ComponentModel.DataAnnotations;

namespace CodeMeet.Api.Contracts.Users;

public record CreateUserRequest(
    string Username,
    string Password,
    [EmailAddress] string Email,
    string? DisplayName
);