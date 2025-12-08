using CodeMeet.Ddd.Domain;

namespace CodeMeet.Domain.Users;

public class User : AggregationRoot
{
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set ;} = string.Empty;
    public string Email { get; private set; } = string.Empty; 

    private User() { } // EF Usage

    private User(string username, string passwordHash, string email)
    {
        Username = username;
        PasswordHash = passwordHash;
        Email = email;
    }

    public static User Create(string username, string passwordHash, string email)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(passwordHash));

        return new User(username, passwordHash, email);
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
    }
}