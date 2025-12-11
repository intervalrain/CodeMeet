using CodeMeet.Application.Common.Security;
using CodeMeet.Ddd.Application.Cqrs.Models;
using ErrorOr;

namespace CodeMeet.Application.Auth.Queries;

public record GenerateTokenQuery(
    Guid UserId,
    string Username,
    string Email,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions) : IQuery<ErrorOr<string>>;

public class GenerateTokenQueryHandler(IJwtTokenGenerator _jwtTokenGenerator)
    : IQueryHandler<GenerateTokenQuery, ErrorOr<string>>
{
    public Task<ErrorOr<string>> HandleAsync(GenerateTokenQuery query, CancellationToken token = default)
    {
        var jwtToken = _jwtTokenGenerator.GenerateToken(
            query.UserId,
            query.Username,
            query.Email,
            query.Roles,
            query.Permissions);

        return Task.FromResult(jwtToken.ToErrorOr());
    }
}