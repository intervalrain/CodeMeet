using CodeMeet.Domain.Matches.Enums;

namespace CodeMeet.Domain.Users.ValueObjects;

public record UserPreferences(
    IReadOnlyList<string> Languages,
    Difficulty Difficulty,
    bool EnableVideo)
{
    public static UserPreferences Default => new([], Difficulty.Medium, true);
}
