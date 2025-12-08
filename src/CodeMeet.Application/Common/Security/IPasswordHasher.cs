namespace CodeMeet.Application.Common.Security;

public interface IPasswordHasher
{
    public string Hash(string password);
}