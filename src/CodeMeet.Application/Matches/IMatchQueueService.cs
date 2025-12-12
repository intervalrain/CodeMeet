using CodeMeet.Application.Matches.Models;
using CodeMeet.Domain.Matches.Enums;

namespace CodeMeet.Application.Matches;

/// <summary>
/// Service for managing the in-memory match queue.
/// </summary>
public interface IMatchQueueService
{
    /// <summary>
    /// Adds a user to the match queue.
    /// </summary>
    Task<QueueEntry> EnqueueAsync(
        Guid userId,
        MatchRole role,
        Difficulty difficulty,
        bool enableVideo,
        CancellationToken ct = default);

    /// <summary>
    /// Removes a user from the queue (user cancellation).
    /// </summary>
    Task<bool> DequeueAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Gets a user's current queue entry if they're waiting.
    /// </summary>
    Task<QueueEntry?> GetEntryAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Gets the number of users ahead of this user in the queue.
    /// Returns 0 if user is first or not in queue.
    /// </summary>
    Task<int> GetAheadCountAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Checks if a user is already in the queue.
    /// </summary>
    Task<bool> IsInQueueAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Finds all compatible pairs in the queue for matching.
    /// Returns pairs with their assigned roles: (Entry, AssignedRole).
    /// </summary>
    Task<IReadOnlyList<MatchPair>> FindCompatiblePairsAsync(CancellationToken ct = default);

    /// <summary>
    /// Removes a matched pair from the queue.
    /// </summary>
    Task RemovePairAsync(Guid userId1, Guid userId2, CancellationToken ct = default);

    /// <summary>
    /// Gets the total number of users in the queue.
    /// </summary>
    Task<int> GetQueueCountAsync(CancellationToken ct = default);
}

/// <summary>
/// Represents a matched pair with assigned roles.
/// </summary>
public record MatchPair(
    QueueEntry Interviewee,
    QueueEntry Interviewer,
    Difficulty CommonDifficulty);
