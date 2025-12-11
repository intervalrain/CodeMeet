namespace CodeMeet.Api.Contracts.Users;

public record UpdateUserPreferencesRequest(
    List<string>? Languages,
    string? Difficulty,
    bool? EnableVideo);