using CodeMeet.Application.Matches;
using Microsoft.Extensions.Logging;

namespace CodeMeet.Infrastructure.Matches;

/// <summary>
/// No-op implementation of IMatchNotificationService.
/// Logs notifications instead of sending them.
/// Will be replaced with SignalR implementation.
/// </summary>
public class NoOpMatchNotificationService(ILogger<NoOpMatchNotificationService> logger)
    : IMatchNotificationService
{
    public Task NotifyMatchFoundAsync(
        Guid matchId,
        Guid intervieweeId,
        Guid interviewerId,
        CancellationToken ct = default)
    {
        logger.LogInformation(
            "Match found: {MatchId} - Interviewee: {IntervieweeId}, Interviewer: {InterviewerId}",
            matchId, intervieweeId, interviewerId);
        return Task.CompletedTask;
    }

    public Task NotifyMatchReadyAsync(
        Guid userId,
        Guid matchId,
        string documentUrl,
        string? videoRoomUrl,
        CancellationToken ct = default)
    {
        logger.LogInformation(
            "Match ready for user {UserId}: {MatchId} - Doc: {DocumentUrl}, Video: {VideoRoomUrl}",
            userId, matchId, documentUrl, videoRoomUrl ?? "N/A");
        return Task.CompletedTask;
    }

    public Task NotifyInsufficientOpportunitiesAsync(Guid userId, CancellationToken ct = default)
    {
        logger.LogInformation(
            "User {UserId} removed from queue: insufficient opportunities",
            userId);
        return Task.CompletedTask;
    }

    public Task NotifyQueueTimeoutAsync(Guid userId, CancellationToken ct = default)
    {
        logger.LogInformation(
            "User {UserId} removed from queue: timeout",
            userId);
        return Task.CompletedTask;
    }
}
