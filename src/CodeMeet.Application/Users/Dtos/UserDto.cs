using CodeMeet.Domain.Users.Entities;

namespace CodeMeet.Application.Users.Dtos;

public record UserDto(Guid Id, string Username, string DisplayName, DateTime CreatedAt)
{
    public static UserDto FromEntity(User user)
    {
        return new UserDto(user.Id, user.Username, user.DisplayName, user.CreatedAt);
    }
}