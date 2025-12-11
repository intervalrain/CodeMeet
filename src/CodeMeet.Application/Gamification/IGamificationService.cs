namespace CodeMeet.Application.Gamification;

/// <summary>
/// Service for managing user interview opportunities.
/// </summary>
public interface IGamificationService
{
    /// <summary>
    /// Gets the current opportunity count for a user.
    /// </summary>
    Task<int> GetOpportunitiesAsync(Guid userId, CancellationToken token = default);
}
