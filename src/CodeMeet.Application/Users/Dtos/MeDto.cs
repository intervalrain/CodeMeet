using CodeMeet.Domain.Users.Entities;

namespace CodeMeet.Application.Users.Dtos;

public record MeDto(
    Guid UserId,
    string Email,
    string DisplayName,
    UserPreferencesDto Preferences,
    int Opportunities,
    DateTime CreatedAt)
{
    public static MeDto FromEntity(User user, int opportunities) => new(
        user.Id,
        user.Email,
        user.DisplayName,
        UserPreferencesDto.FromValueObject(user.Preferences),
        opportunities,
        user.CreatedAt);
}