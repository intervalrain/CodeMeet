using CodeMeet.Domain.Matches.Enums;

namespace CodeMeet.Application.Matches.Models;

/// <summary>
/// Represents a user waiting in the match queue.
/// This is a transient in-memory structure, NOT a domain entity.
/// </summary>
public record QueueEntry(
    Guid QueueId,
    Guid UserId,
    MatchRole Role,
    Difficulty Difficulty,
    bool EnableVideo,
    DateTime EnteredAt);
