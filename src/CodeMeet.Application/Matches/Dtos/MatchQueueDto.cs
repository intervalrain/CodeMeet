using CodeMeet.Domain.Matches.Enums;

namespace CodeMeet.Application.Matches.Dtos;

public record MatchQueueDto(
    Guid QueueId,
    QueueStatus Status,
    int AheadCount,
    DateTime EnteredAt
);