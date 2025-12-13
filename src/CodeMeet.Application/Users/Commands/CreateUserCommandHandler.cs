using System.ComponentModel.DataAnnotations;
using CodeMeet.Application.Auth.Dtos;
using CodeMeet.Application.Common.Security;
using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Auth.Entities;
using CodeMeet.Domain.Users.Entities;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace CodeMeet.Application.Users.Commands;

public record CreateUserCommand(string Username, string Password, [EmailAddress]string Email, string? DisplayName) : ICommand<ErrorOr<AuthDto>>;

public class CreateUserCommandHandler(
    IPasswordHasher hasher,
    IRepository<User> userRepository,
    IRepository<RefreshToken> refreshTokenRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IOptions<JwtSettings> jwtOptions) : ICommandHandler<CreateUserCommand, ErrorOr<AuthDto>>
{
    public async Task<ErrorOr<AuthDto>> HandleAsync(CreateUserCommand command, CancellationToken token = default)
    {
        var username = command.Username;
        var email = command.Email;

        var usernameExisted = await userRepository.AnyAsync(user => user.Username == username, token);
        if (usernameExisted)
        {
            return Error.Conflict(description: "The username has been registered.");
        }

        var emailExisted = await userRepository.AnyAsync(user => user.Email == email, token);
        if (emailExisted)
        {
            return Error.Conflict(description: "The email has been registered.");
        }

        var passwordHash = hasher.Hash(command.Password);

        var user = User.Create(username, passwordHash, email, command.DisplayName);
        var jwtToken = jwtTokenGenerator.GenerateToken(user.Id, username, email, user.Roles, user.Permissions);

        // Generate refresh token
        var refreshTokenString = refreshTokenGenerator.GenerateToken();
        var refreshTokenHash = refreshTokenGenerator.HashToken(refreshTokenString);
        var refreshToken = RefreshToken.Create(
            user.Id,
            refreshTokenHash,
            jwtOptions.Value.RefreshTokenExpirationInDays);

        await userRepository.InsertAsync(user, token);
        await refreshTokenRepository.InsertAsync(refreshToken, token);

        return new AuthDto(UserDto.FromEntity(user), jwtToken, refreshTokenString);
    }
}