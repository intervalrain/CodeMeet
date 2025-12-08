using CodeMeet.Application.Common.Security;
using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Authorization;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users;
using ErrorOr;

namespace CodeMeet.Application.Users.Commands;

public record UpdateUserCommand(Guid UserId, string Password, string NewPassword) : IAuthorizeableCommand<ErrorOr<UserDto>>;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, ErrorOr<UserDto>>
{
    private readonly IRepository<User> _repository;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateUserCommandHandler(IRepository<User> repository, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    public async Task<ErrorOr<UserDto>> HandleAsync(UpdateUserCommand command, CancellationToken token = default)
    {
        var user = await _repository.FindAsync(command.UserId, token);
        if (user is null)
        {
            return Error.NotFound(description: "User not found");
        }

        if (!_passwordHasher.Verify(user.PasswordHash, command.Password))
        {
            return Error.Unauthorized(description: "Username or password not correct");
        }

        user.ChangePassword(_passwordHasher.Hash(command.NewPassword));

        await _repository.UpdateAsync(user);

        return new UserDto(user.Id, user.Username).ToErrorOr();
    }
}