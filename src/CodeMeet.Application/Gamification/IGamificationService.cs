namespace CodeMeet.Application.Gamification;

/// <summary>
/// Service for managing user interview opportunities (tokens).
/// </summary>
public interface IGamificationService
{
    /// <summary>
    /// Gets the current opportunity count for a user.
    /// </summary>
    Task<int> GetOpportunitiesAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Checks if user has at least one opportunity available.
    /// </summary>
    Task<bool> HasOpportunityAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Attempts to consume one opportunity. Returns true if successful.
    /// Called when interview starts (interviewee consumes).
    /// </summary>
    Task<bool> TryConsumeAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Awards opportunities to a user.
    /// Called when interview completes (interviewer earns).
    /// </summary>
    Task AwardAsync(Guid userId, int amount = 1, CancellationToken ct = default);

    /// <summary>
    /// Attempts to award the daily free opportunity.
    /// Returns true if awarded, false if already received today.
    /// </summary>
    Task<bool> TryAwardDailyAsync(Guid userId, CancellationToken ct = default);
}