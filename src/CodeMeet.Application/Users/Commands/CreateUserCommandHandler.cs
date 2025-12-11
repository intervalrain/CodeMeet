using System.ComponentModel.DataAnnotations;
using CodeMeet.Application.Auth.Dtos;
using CodeMeet.Application.Common.Security;
using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users.Entities;
using ErrorOr;

namespace CodeMeet.Application.Users.Commands;

public record CreateUserCommand(string Username, string Password, [EmailAddress]string Email, string? DisplayName) : ICommand<ErrorOr<AuthDto>>;

public class CreateUserCommandHandler(IPasswordHasher hasher, IRepository<User> repository, IJwtTokenGenerator jwtTokenGenerator) : ICommandHandler<CreateUserCommand, ErrorOr<AuthDto>>
{
    public async Task<ErrorOr<AuthDto>> HandleAsync(CreateUserCommand command, CancellationToken token = default)
    {
        var username = command.Username;
        var email = command.Email;
        
        var usernameExisted = await repository.AnyAsync(user => user.Username == username, token);
        if (usernameExisted)
        {
            return Error.Conflict(description: "The username has been registered.");
        }

        var emailExisted = await repository.AnyAsync(user => user.Email == email, token);
        if (emailExisted)
        {
            return Error.Conflict(description: "The email has been registered.");
        }

        var passwordHash = hasher.Hash(command.Password);
        
        var user = User.Create(username, passwordHash, email, command.DisplayName);
        var jwtToken = jwtTokenGenerator.GenerateToken(user.Id, username, email);

        await repository.InsertAsync(user);

        return new AuthDto(UserDto.FromEntity(user), jwtToken).ToErrorOr();   
    }
}