using CodeMeet.Ddd.Domain;
using CodeMeet.Domain.Matches.Enums;

namespace CodeMeet.Domain.Matches.Events;

public record MatchCreatedEvent(
    Guid MatchId,
    Guid IntervieweeId,
    Guid InterviewerId,
    Difficulty Difficulty,
    bool EnableVideo,
    int? SuggestedQuestionId) : DomainEvent;
