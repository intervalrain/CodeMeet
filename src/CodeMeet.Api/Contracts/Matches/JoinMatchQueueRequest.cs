using CodeMeet.Domain.Matches.Enums;

namespace CodeMeet.Api.Contracts.Matches;

public record JoinMatchQueueRequest(
    MatchRole Role,
    Difficulty Difficulty,
    bool EnableVideo);