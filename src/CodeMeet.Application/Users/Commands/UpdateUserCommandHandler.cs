using CodeMeet.Application.Common.Security;
using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Authorization;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users.Entities;
using ErrorOr;

namespace CodeMeet.Application.Users.Commands;

public record UpdateUserCommand(Guid UserId, string Password, string NewPassword) : IAuthorizeableCommand<ErrorOr<UserDto>>;

public class UpdateUserCommandHandler(IRepository<User> repository, IPasswordHasher passwordHasher) : ICommandHandler<UpdateUserCommand, ErrorOr<UserDto>>
{
    public async Task<ErrorOr<UserDto>> HandleAsync(UpdateUserCommand command, CancellationToken token = default)
    {
        var user = await repository.FindAsync(command.UserId, token);
        if (user is null)
        {
            return Error.NotFound(description: "User not found");
        }

        if (!passwordHasher.Verify(user.PasswordHash, command.Password))
        {
            return Error.Unauthorized(description: "Username or password not correct");
        }

        user.ChangePassword(passwordHasher.Hash(command.NewPassword));

        await repository.UpdateAsync(user);

        return UserDto.FromEntity(user);
    }
}