using System.ComponentModel.DataAnnotations;

namespace CodeMeet.Api.Models.Auth;

public record RegisterDto(
    string Username,
    string Password,
    [EmailAddress] string Email
);