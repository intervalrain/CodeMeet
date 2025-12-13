using CodeMeet.Application.Common.Security;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Auth.Entities;
using ErrorOr;

namespace CodeMeet.Application.Auth.Commands;

public record RevokeTokenCommand(string RefreshToken) : ICommand<ErrorOr<Unit>>;

public class RevokeTokenCommandHandler(
    IRefreshTokenGenerator refreshTokenGenerator,
    IRepository<RefreshToken> refreshTokenRepository) : ICommandHandler<RevokeTokenCommand, ErrorOr<Unit>>
{
    public async Task<ErrorOr<Unit>> HandleAsync(RevokeTokenCommand command, CancellationToken token = default)
    {
        // Hash the incoming refresh token
        var tokenHash = refreshTokenGenerator.HashToken(command.RefreshToken);

        // Find the refresh token in the database
        var refreshToken = await refreshTokenRepository.FindAsync(
            rt => rt.TokenHash == tokenHash,
            token);

        // Idempotent: if token doesn't exist or already revoked, consider it successful
        if (refreshToken is null || refreshToken.IsRevoked)
        {
            return Unit.Value;
        }

        // Revoke the token
        refreshToken.Revoke();
        await refreshTokenRepository.UpdateAsync(refreshToken, token);

        return Unit.Value;
    }
}