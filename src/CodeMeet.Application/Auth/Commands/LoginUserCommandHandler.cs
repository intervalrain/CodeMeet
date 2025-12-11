using CodeMeet.Application.Auth.Dtos;
using CodeMeet.Application.Common.Security;
using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users.Entities;

using ErrorOr;

namespace CodeMeet.Application.Auth.Commands;

public record LoginUserCommand(string Username, string Password) : ICommand<ErrorOr<AuthDto>>;

public class LoginUserCommandHandler(IPasswordHasher hasher, IRepository<User> repository, IJwtTokenGenerator jwtTokenGenerator) : ICommandHandler<LoginUserCommand, ErrorOr<AuthDto>>
{
    public async Task<ErrorOr<AuthDto>> HandleAsync(LoginUserCommand command, CancellationToken token = default)
    {
        var username = command.Username;
        var user = await repository.FindAsync(user => user.Username == username, token);
        if (user is null)
        {
            return Error.NotFound(description: "The user is not found.");
        }

        var valid = hasher.Verify(user.PasswordHash, command.Password);
        if (!valid)
        {
            return Error.Unauthorized(description: "The uesrname or password is not correct.");
        }
        
        var jwtToken = jwtTokenGenerator.GenerateToken(user.Id, username, user.Email, user.Roles, user.Permissions);

        return new AuthDto(UserDto.FromEntity(user), jwtToken).ToErrorOr();
    }
}