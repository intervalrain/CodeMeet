using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Authorization;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Matches.Enums;
using CodeMeet.Domain.Users.Entities;
using CodeMeet.Domain.Users.ValueObjects;

using ErrorOr;

namespace CodeMeet.Application.Users.Commands;

public record UpdateUserPreferencesCommand(Guid UserId, List<string>? Languages, Difficulty? Difficulty, bool? EnableVideo) : IAuthorizeableCommand<ErrorOr<UserPreferencesDto>>;

public class UpdateUserPreferencesCommandHandler(IRepository<User> repository) : ICommandHandler<UpdateUserPreferencesCommand, ErrorOr<UserPreferencesDto>>
{
    public async Task<ErrorOr<UserPreferencesDto>> HandleAsync(UpdateUserPreferencesCommand command, CancellationToken token = default)
    {
        var user = await repository.FindAsync(command.UserId, token);
        if (user is null)
        {
            return Error.NotFound(description: "User not found");
        }
        var preferences = user.Preferences;
        var languages = command.Languages ?? preferences.Languages;
        var difficulty = command.Difficulty ?? preferences.Difficulty;
        var enableVideo = command.EnableVideo ?? preferences.EnableVideo;

        user.UpdatePreferences(new UserPreferences(languages, difficulty, enableVideo));
        
        await repository.UpdateAsync(user, token);
        
        return UserPreferencesDto.FromValueObject(user.Preferences);
    }
}