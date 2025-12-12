using System.Text.Json.Serialization;
using CodeMeet.Ddd.Domain;

namespace CodeMeet.Domain.Gamification.Entities;

/// <summary>
/// Tracks a user's interview opportunities (tokens).
/// Separate from User aggregate to maintain single responsibility.
/// </summary>
public class UserOpportunity : AggregationRoot
{
    public Guid UserId { get; private set; }
    public int Balance { get; private set; }
    public DateTime? LastDailyAwardAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private UserOpportunity() { } // EF

    [JsonConstructor]
    private UserOpportunity(
        Guid userId,
        int balance,
        DateTime? lastDailyAwardAt,
        DateTime createdAt,
        DateTime updatedAt)
    {
        UserId = userId;
        Balance = balance;
        LastDailyAwardAt = lastDailyAwardAt;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    /// <summary>
    /// Creates a new UserOpportunity with initial balance.
    /// </summary>
    public static UserOpportunity Create(Guid userId, int initialBalance = 1)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (initialBalance < 0)
            throw new ArgumentException("Initial balance cannot be negative", nameof(initialBalance));

        return new UserOpportunity
        {
            UserId = userId,
            Balance = initialBalance,
            LastDailyAwardAt = DateTime.UtcNow, // First creation counts as daily award
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Attempts to consume one opportunity. Returns false if insufficient balance.
    /// </summary>
    public bool TryConsume()
    {
        if (Balance <= 0)
            return false;

        Balance--;
        UpdatedAt = DateTime.UtcNow;
        return true;
    }

    /// <summary>
    /// Awards opportunities to the user.
    /// </summary>
    public void Award(int amount = 1)
    {
        if (amount <= 0)
            throw new ArgumentException("Award amount must be positive", nameof(amount));

        Balance += amount;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Attempts to award the daily free opportunity.
    /// Returns false if already awarded today.
    /// </summary>
    public bool TryAwardDaily()
    {
        var today = DateTime.UtcNow.Date;

        if (LastDailyAwardAt.HasValue && LastDailyAwardAt.Value.Date >= today)
            return false;

        Balance++;
        LastDailyAwardAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        return true;
    }

    /// <summary>
    /// Checks if the user has at least one opportunity available.
    /// </summary>
    public bool HasOpportunity() => Balance > 0;
}