using System.Text.Json.Serialization;
using CodeMeet.Ddd.Domain;
using CodeMeet.Domain.Users.Events;
using CodeMeet.Domain.Users.ValueObjects;

namespace CodeMeet.Domain.Users.Entities;

public class User : AggregationRoot
{
    public string Username { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string DisplayName { get; private set; } = string.Empty;
    public List<string> Roles { get; private set; } = [];
    public List<string> Permissions { get; private set; } = [];
    public UserPreferences Preferences { get; private set; } = UserPreferences.Default;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private User() { } // EF

    [JsonConstructor]
    private User(
        string username,
        string passwordHash,
        string email,
        string displayName,
        List<string>? roles,
        List<string>? permissions,
        UserPreferences? preferences,
        DateTime createdAt,
        DateTime updatedAt)
    {
        ArgumentException.ThrowIfNullOrEmpty(username);
        ArgumentException.ThrowIfNullOrEmpty(passwordHash);
        ArgumentException.ThrowIfNullOrEmpty(email);
        ArgumentException.ThrowIfNullOrEmpty(displayName);
        Username = username;
        PasswordHash = passwordHash;
        Email = email;
        DisplayName = displayName;
        Roles = roles ?? [];
        Permissions = permissions ?? [];
        Preferences = preferences ?? UserPreferences.Default;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

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

    public void ChangeDisplayName(string newDisplayName)
    {
        DisplayName = newDisplayName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePreferences(UserPreferences preferences)
    {
        Preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        UpdatedAt = DateTime.UtcNow;
    }
}