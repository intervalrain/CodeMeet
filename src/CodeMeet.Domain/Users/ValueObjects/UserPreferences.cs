namespace CodeMeet.Domain.Users.ValueObjects;

public record UserPreferences(
    IReadOnlyList<string> Languages,
    string Difficulty,
    bool EnableVideo)
{
    public static UserPreferences Default => new([], "Medium", true);
}
