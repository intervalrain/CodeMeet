namespace CodeMeet.Application.Common.Security;

public interface IJwtTokenGenerator
{
    string GenerateToken(
        Guid id,
        string username,
        string email,
        IReadOnlyList<string> roles,
        IReadOnlyList<string> permissions);
}