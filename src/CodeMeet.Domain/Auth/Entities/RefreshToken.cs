using CodeMeet.Ddd.Domain;

namespace CodeMeet.Domain.Auth.Entities;

public class RefreshToken : AggregationRoot
{
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime? RevokedAt { get; private set; }

    private RefreshToken() { } // EF

    private RefreshToken(Guid userId, string tokenHash, DateTime expiresAt)
    {
        UserId = userId;
        TokenHash = tokenHash;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
        IsRevoked = false;
    }

    public static RefreshToken Create(Guid userId, string tokenHash, int expirationInDays)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));
        
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentException("Token hash cannot be empty", nameof(tokenHash));

        if (expirationInDays <= 0)
            throw new ArgumentException("Expiration days must be positive", nameof(expirationInDays));

        var expiresAt = DateTime.UtcNow.AddDays(expirationInDays);
        return new RefreshToken(userId, tokenHash, expiresAt);
    }

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;

    public bool IsValid() => !IsRevoked && !IsExpired();

    public void Revoke()
    {
        if (IsRevoked)
            throw new InvalidOperationException("Token is already revoked");

        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }
}