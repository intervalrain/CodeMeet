using CodeMeet.Application.Auth.Dtos;
using CodeMeet.Application.Common.Security;
using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users.Entities;

using ErrorOr;

namespace CodeMeet.Application.Auth.Commands;

public record LoginUserCommand(string Username, string Password) : ICommand<ErrorOr<AuthDto>>;

public class LoginUserCommandHandler : ICommandHandler<LoginUserCommand, ErrorOr<AuthDto>>
{
    private readonly IPasswordHasher _hasher;
    private readonly IRepository<User> _repository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginUserCommandHandler(IPasswordHasher hasher, IRepository<User> repository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _hasher = hasher;
        _repository = repository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ErrorOr<AuthDto>> HandleAsync(LoginUserCommand command, CancellationToken token = default)
    {
        var username = command.Username;
        var user = await _repository.FindAsync(user => user.Username == username, token);
        if (user is null)
        {
            return Error.NotFound(description: "The user is not found.");
        }

        var valid = _hasher.Verify(user.PasswordHash, command.Password);
        if (!valid)
        {
            return Error.Unauthorized(description: "The uesrname or password is not correct.");
        }
        
        var jwtToken = _jwtTokenGenerator.GenerateToken(user.Id, username, user.Email);

        return new AuthDto(UserDto.FromEntity(user), jwtToken).ToErrorOr();
    }
}