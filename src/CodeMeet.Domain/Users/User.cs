using CodeMeet.Ddd.Domain;

namespace CodeMeet.Domain.Users;

public class User : AggregationRoot
{
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set ;} = string.Empty;

    private User() { } // EF Usage

    private User(string username, string passwordHash)
    {
        Username = username;
        PasswordHash = passwordHash;
    }

    public static User Create(string username, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        return new User(username, passwordHash);
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
    }
}