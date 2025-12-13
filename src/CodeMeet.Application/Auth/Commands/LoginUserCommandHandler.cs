using CodeMeet.Application.Auth.Dtos;
using CodeMeet.Application.Common.Security;
using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Auth.Entities;
using CodeMeet.Domain.Users.Entities;
using ErrorOr;
using Microsoft.Extensions.Options;

namespace CodeMeet.Application.Auth.Commands;

public record LoginUserCommand(string Username, string Password) : ICommand<ErrorOr<AuthDto>>;

public class LoginUserCommandHandler(
    IPasswordHasher hasher,
    IRepository<User> userRepository,
    IRepository<RefreshToken> refreshTokenRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IRefreshTokenGenerator refreshTokenGenerator,
    IOptions<JwtSettings> jwtOptions) : ICommandHandler<LoginUserCommand, ErrorOr<AuthDto>>
{
    public async Task<ErrorOr<AuthDto>> HandleAsync(LoginUserCommand command, CancellationToken token = default)
    {
        var username = command.Username;
        var user = await userRepository.FindAsync(user => user.Username == username, token);
        if (user is null)
        {
            return Error.NotFound(description: "The user is not found.");
        }

        var valid = hasher.Verify(user.PasswordHash, command.Password);
        if (!valid)
        {
            return Error.Unauthorized(description: "The uesrname or password is not correct.");
        }
        
        // Generate access token
        var jwtToken = jwtTokenGenerator.GenerateToken(user.Id, username, user.Email, user.Roles, user.Permissions);

        // Generate refresh token
        var refreshTokenString = refreshTokenGenerator.GenerateToken();
        var refreshTokenHash = refreshTokenGenerator.HashToken(refreshTokenString);
        var refreshToken = RefreshToken.Create(
            user.Id,
            refreshTokenHash,
            jwtOptions.Value.RefreshTokenExpirationInDays);

        await refreshTokenRepository.InsertAsync(refreshToken, token);

        return new AuthDto(UserDto.FromEntity(user), jwtToken, refreshTokenString);
    }
}