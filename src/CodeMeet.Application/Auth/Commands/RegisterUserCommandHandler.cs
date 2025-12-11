using System.ComponentModel.DataAnnotations;
using CodeMeet.Application.Auth.Dtos;
using CodeMeet.Application.Common.Security;
using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users.Entities;
using ErrorOr;

namespace CodeMeet.Application.Auth.Commands;

public record RegisterUserCommand(string Username, string Password, [EmailAddress]string Email) : ICommand<ErrorOr<AuthDto>>;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, ErrorOr<AuthDto>>
{
    private readonly IPasswordHasher _hasher;
    private readonly IRepository<User> _repository;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterUserCommandHandler(IPasswordHasher hasher, IRepository<User> repository, IJwtTokenGenerator jwtTokenGenerator)
    {
        _hasher = hasher;
        _repository = repository;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<ErrorOr<AuthDto>> HandleAsync(RegisterUserCommand command, CancellationToken token = default)
    {
        var username = command.Username;
        var existed = await _repository.AnyAsync(user => user.Username == username, token);
        if (existed)
        {
            return Error.Conflict(description: "The username has been registered.");
        }

        var passwordHash = _hasher.Hash(command.Password);
        var email = command.Email;
        
        var user = User.Create(username, passwordHash, email);
        var jwtToken = _jwtTokenGenerator.GenerateToken(user.Id, username, email);

        await _repository.InsertAsync(user);

        return new AuthDto(UserDto.FromEntity(user), jwtToken).ToErrorOr();   
    }
}