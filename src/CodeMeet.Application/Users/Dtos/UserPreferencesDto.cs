using CodeMeet.Domain.Matches.Enums;
using CodeMeet.Domain.Users.ValueObjects;

namespace CodeMeet.Application.Users.Dtos;

public record UserPreferencesDto(
    IReadOnlyList<string> Languages,
    Difficulty Difficulty,
    bool EnableVideo)
{
    public static UserPreferencesDto FromValueObject(UserPreferences preferences) => new(
        preferences.Languages,
        preferences.Difficulty,
        preferences.EnableVideo);
}
