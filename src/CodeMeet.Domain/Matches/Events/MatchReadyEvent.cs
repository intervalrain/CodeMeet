using CodeMeet.Ddd.Domain;

namespace CodeMeet.Domain.Matches.Events;

public record MatchReadyEvent(
    Guid MatchId,
    string DocumentUrl,
    string? VideoRoomUrl) : DomainEvent;