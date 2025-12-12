using System.Collections.Concurrent;
using CodeMeet.Application.Matches;
using CodeMeet.Application.Matches.Models;
using CodeMeet.Domain.Matches.Enums;

namespace CodeMeet.Infrastructure.Matches;

/// <summary>
/// In-memory implementation of the match queue service.
/// Uses ConcurrentDictionary for thread-safety.
/// </summary>
public class InMemoryMatchQueueService : IMatchQueueService
{
    private readonly ConcurrentDictionary<Guid, QueueEntry> _queue = new();
    private readonly SemaphoreSlim _matchLock = new(1, 1);

    public Task<QueueEntry> EnqueueAsync(
        Guid userId,
        MatchRole role,
        Difficulty difficulty,
        bool enableVideo,
        CancellationToken ct = default)
    {
        var entry = new QueueEntry(
            QueueId: Guid.NewGuid(),
            UserId: userId,
            Role: role,
            Difficulty: difficulty,
            EnableVideo: enableVideo,
            EnteredAt: DateTime.UtcNow);

        if (!_queue.TryAdd(userId, entry))
        {
            throw new InvalidOperationException("User is already in the queue");
        }

        return Task.FromResult(entry);
    }

    public Task<bool> DequeueAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult(_queue.TryRemove(userId, out _));
    }

    public Task<QueueEntry?> GetEntryAsync(Guid userId, CancellationToken ct = default)
    {
        _queue.TryGetValue(userId, out var entry);
        return Task.FromResult(entry);
    }

    public Task<int> GetAheadCountAsync(Guid userId, CancellationToken ct = default)
    {
        if (!_queue.TryGetValue(userId, out var entry))
        {
            return Task.FromResult(0);
        }

        // Count users who entered before this user (strictly less than)
        var aheadCount = _queue.Values
            .Count(e => e.EnteredAt < entry.EnteredAt);

        return Task.FromResult(aheadCount);
    }

    public Task<bool> IsInQueueAsync(Guid userId, CancellationToken ct = default)
    {
        return Task.FromResult(_queue.ContainsKey(userId));
    }

    public async Task<IReadOnlyList<MatchPair>> FindCompatiblePairsAsync(CancellationToken ct = default)
    {
        await _matchLock.WaitAsync(ct);
        try
        {
            var pairs = new List<MatchPair>();
            var matched = new HashSet<Guid>();

            // Sort by EnteredAt for FIFO priority
            var entries = _queue.Values
                .OrderBy(e => e.EnteredAt)
                .ToList();

            for (var i = 0; i < entries.Count; i++)
            {
                if (matched.Contains(entries[i].UserId))
                    continue;

                for (var j = i + 1; j < entries.Count; j++)
                {
                    if (matched.Contains(entries[j].UserId))
                        continue;

                    var pair = TryMatch(entries[i], entries[j]);
                    if (pair is not null)
                    {
                        pairs.Add(pair);
                        matched.Add(entries[i].UserId);
                        matched.Add(entries[j].UserId);
                        break;
                    }
                }
            }

            return pairs;
        }
        finally
        {
            _matchLock.Release();
        }
    }

    public Task RemovePairAsync(Guid userId1, Guid userId2, CancellationToken ct = default)
    {
        _queue.TryRemove(userId1, out _);
        _queue.TryRemove(userId2, out _);
        return Task.CompletedTask;
    }

    public Task<int> GetQueueCountAsync(CancellationToken ct = default)
    {
        return Task.FromResult(_queue.Count);
    }

    private MatchPair? TryMatch(QueueEntry a, QueueEntry b)
    {
        // Rule 1: Cannot match with self
        if (a.UserId == b.UserId)
            return null;

        // Rule 2: Role compatibility
        var (interviewee, interviewer) = DetermineRoles(a, b);
        if (interviewee is null || interviewer is null)
            return null;

        // Rule 3: Difficulty overlap (flags intersection)
        var commonDifficulty = a.Difficulty & b.Difficulty;
        if (commonDifficulty == 0)
            return null;

        // Rule 4: Video preference must match
        if (a.EnableVideo != b.EnableVideo)
            return null;

        // Select a single difficulty from the common ones
        var selectedDifficulty = SelectDifficulty(commonDifficulty);

        return new MatchPair(interviewee, interviewer, selectedDifficulty);
    }

    private (QueueEntry? Interviewee, QueueEntry? Interviewer) DetermineRoles(QueueEntry a, QueueEntry b)
    {
        var aCanBeInterviewee = a.Role.HasFlag(MatchRole.Interviewee);
        var aCanBeInterviewer = a.Role.HasFlag(MatchRole.Interviewer);
        var bCanBeInterviewee = b.Role.HasFlag(MatchRole.Interviewee);
        var bCanBeInterviewer = b.Role.HasFlag(MatchRole.Interviewer);

        // Case 1: A only wants Interviewee, B can be Interviewer
        if (aCanBeInterviewee && !aCanBeInterviewer && bCanBeInterviewer)
            return (a, b);

        // Case 2: A only wants Interviewer, B can be Interviewee
        if (aCanBeInterviewer && !aCanBeInterviewee && bCanBeInterviewee)
            return (b, a);

        // Case 3: B only wants Interviewee, A can be Interviewer
        if (bCanBeInterviewee && !bCanBeInterviewer && aCanBeInterviewer)
            return (b, a);

        // Case 4: B only wants Interviewer, A can be Interviewee
        if (bCanBeInterviewer && !bCanBeInterviewee && aCanBeInterviewee)
            return (a, b);

        // Case 5: Both are flexible (Both) - randomly assign
        if (aCanBeInterviewee && aCanBeInterviewer && bCanBeInterviewee && bCanBeInterviewer)
        {
            // Use Random to decide
            return Random.Shared.Next(2) == 0 ? (a, b) : (b, a);
        }

        // Case 6: A=Both, B=Interviewee only -> A=Interviewer, B=Interviewee
        if (aCanBeInterviewer && bCanBeInterviewee && !bCanBeInterviewer)
            return (b, a);

        // Case 7: A=Both, B=Interviewer only -> A=Interviewee, B=Interviewer
        if (aCanBeInterviewee && bCanBeInterviewer && !bCanBeInterviewee)
            return (a, b);

        // No compatible role assignment found
        return (null, null);
    }

    private Difficulty SelectDifficulty(Difficulty common)
    {
        // Select one difficulty from the flags, preferring Medium > Easy > Hard
        if (common.HasFlag(Difficulty.Medium))
            return Difficulty.Medium;
        if (common.HasFlag(Difficulty.Easy))
            return Difficulty.Easy;
        if (common.HasFlag(Difficulty.Hard))
            return Difficulty.Hard;

        return common;
    }
}
