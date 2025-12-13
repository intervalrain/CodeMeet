namespace CodeMeet.Application.Common.Security;

public interface IRefreshTokenGenerator
{
    string GenerateToken();
    string HashToken(string token);
}