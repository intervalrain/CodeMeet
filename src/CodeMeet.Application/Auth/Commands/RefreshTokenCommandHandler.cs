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

public record RefreshTokenCommand(string RefreshToken) : ICommand<ErrorOr<AuthDto>>;

public class RefreshTokenCommandHandler(
    IRefreshTokenGenerator refreshTokenGenerator,
    IRepository<RefreshToken> refreshTokenRepository,
    IRepository<User> userRepository,
    IJwtTokenGenerator jwtTokenGenerator,
    IOptions<JwtSettings> jwtOptions) : ICommandHandler<RefreshTokenCommand, ErrorOr<AuthDto>>
{
    public async Task<ErrorOr<AuthDto>> HandleAsync(RefreshTokenCommand command, CancellationToken token = default)
    {
        // Hash the incoming refresh token
        var tokenHash = refreshTokenGenerator.HashToken(command.RefreshToken);

        // Find the refresh token in the database
        var refreshToken = await refreshTokenRepository.FindAsync(
            rt => rt.TokenHash == tokenHash,
            token);

        if (refreshToken is null)
        {
            return Error.Unauthorized(description: "Invalid refresh token");
        }

        // Validate the refresh token
        if (!refreshToken.IsValid())
        {
            return Error.Unauthorized(description: "Refresh token is expired or revoked");
        }

        // Get the user
        var user = await userRepository.FindAsync(refreshToken.UserId, token);
        if (user is null)
        {
            return Error.NotFound(description: "User not found");
        }

        // Revoke the old refresh token (token rotation)
        refreshToken.Revoke();
        await refreshTokenRepository.UpdateAsync(refreshToken, token);

        // Generate new tokens
        var newAccessToken = jwtTokenGenerator.GenerateToken(
            user.Id,
            user.Username,
            user.Email,
            user.Roles,
            user.Permissions);

        var newRefreshTokenString = refreshTokenGenerator.GenerateToken();
        var newRefreshTokenHash = refreshTokenGenerator.HashToken(newRefreshTokenString);
        var newRefreshToken = RefreshToken.Create(
            user.Id,
            newRefreshTokenHash,
            jwtOptions.Value.RefreshTokenExpirationInDays);

        await refreshTokenRepository.InsertAsync(newRefreshToken, token);

        return new AuthDto(
            UserDto.FromEntity(user),
            newAccessToken,
            newRefreshTokenString);
    }
}