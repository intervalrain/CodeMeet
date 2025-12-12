using CodeMeet.Domain.Matches.Enums;

namespace CodeMeet.Api.Contracts.Users;

public record UpdateUserPreferencesRequest(
    List<string>? Languages,
    Difficulty? Difficulty,
    bool? EnableVideo);