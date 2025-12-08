using CodeMeet.Application.Common.Security;

namespace CodeMeet.Infrastructure.Common.Security;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return password;
    }
}