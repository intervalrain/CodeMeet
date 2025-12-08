using CodeMeet.Application.Common.Security;
using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users;
using ErrorOr;

namespace CodeMeet.Application.Users.Commands;

public record RegisterUserCommand(string Username, string Password) : ICommand<ErrorOr<UserDto>>;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, ErrorOr<UserDto>>
{
    private readonly IPasswordHasher _hasher;
    private readonly IRepository<User> _repository;

    public RegisterUserCommandHandler(IPasswordHasher hasher, IRepository<User> repository)
    {
        _hasher = hasher;
        _repository = repository;
    }

    public async Task<ErrorOr<UserDto>> HandleAsync(RegisterUserCommand command, CancellationToken token = default)
    {
        var username = command.Username;
        // var existed = _repository.FindAsync(u => u)

        var passwordHash = _hasher.Hash(command.Password);
        
        var user = User.Create(username, passwordHash);
        await _repository.InsertAsync(user);

        return new UserDto(user.Id, user.Username).ToErrorOr();   
    }
}