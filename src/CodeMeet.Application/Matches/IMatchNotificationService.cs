namespace CodeMeet.Application.Matches;

/// <summary>
/// Service for sending real-time match notifications.
/// Currently a no-op stub; will be replaced with SignalR implementation.
/// </summary>
public interface IMatchNotificationService
{
    /// <summary>
    /// Notifies users that a match was found.
    /// </summary>
    Task NotifyMatchFoundAsync(
        Guid matchId,
        Guid intervieweeId,
        Guid interviewerId,
        CancellationToken ct = default);

    /// <summary>
    /// Notifies a user that the match is ready with resources.
    /// </summary>
    Task NotifyMatchReadyAsync(
        Guid userId,
        Guid matchId,
        string documentUrl,
        string? videoRoomUrl,
        CancellationToken ct = default);

    /// <summary>
    /// Notifies a user they were removed from queue due to insufficient opportunities.
    /// </summary>
    Task NotifyInsufficientOpportunitiesAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Notifies a user their queue wait timed out.
    /// </summary>
    Task NotifyQueueTimeoutAsync(Guid userId, CancellationToken ct = default);
}
