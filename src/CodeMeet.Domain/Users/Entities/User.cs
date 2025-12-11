using CodeMeet.Ddd.Domain;
using CodeMeet.Domain.Users.Events;
using CodeMeet.Domain.Users.ValueObjects;

namespace CodeMeet.Domain.Users.Entities;

public class User : AggregationRoot
{
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public List<string> Roles { get; set; }= [];
    public List<string> Permissions { get; set; }= [];
    public UserPreferences Preferences { get; private set; } = UserPreferences.Default;
    public DateTime CreatedAt { get; }
    public DateTime UpdatedAt { get; private set; }

    private User() { } // EF Usage

    private User(string username, string passwordHash, string email, string? displayName)
    {
        Username = username;
        PasswordHash = passwordHash;
        Email = email;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        DisplayName = displayName ?? username;

        AddDomainEvent(new UserCreatedEvent(Username, Email, DisplayName));
    }

    public static User Create(string username, string passwordHash, string email, string? displayName)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(passwordHash));

        return new User(username, passwordHash, email, displayName);
    }

    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePreferences(UserPreferences preferences)
    {
        Preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        UpdatedAt = DateTime.UtcNow;
    }
}