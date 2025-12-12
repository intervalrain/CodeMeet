using CodeMeet.Application.Gamification;
using CodeMeet.Application.Matches.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Authorization;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Domain.Matches.Enums;
using ErrorOr;

namespace CodeMeet.Application.Matches.Commands;

public record JoinMatchQueueCommand(
    Guid UserId,
    MatchRole Role,
    Difficulty Difficulty,
    bool EnableVideo) : IAuthorizeableCommand<ErrorOr<MatchQueueDto>>;

public class JoinMatchQueueCommandHandler(
    IMatchQueueService queueService,
    IGamificationService gamificationService)
    : ICommandHandler<JoinMatchQueueCommand, ErrorOr<MatchQueueDto>>
{
    public async Task<ErrorOr<MatchQueueDto>> HandleAsync(
        JoinMatchQueueCommand command,
        CancellationToken token = default)
    {
        // 1. Check if user is already in queue
        if (await queueService.IsInQueueAsync(command.UserId, token))
        {
            return Error.Conflict(description: "You are already in the match queue");
        }

        // 2. If user wants to be interviewee, check if they have opportunities
        if (command.Role.HasFlag(MatchRole.Interviewee))
        {
            var hasOpportunity = await gamificationService.HasOpportunityAsync(command.UserId, token);
            if (!hasOpportunity)
            {
                return Error.Validation(
                    code: "InsufficientOpportunities",
                    description: "You need at least 1 opportunity to join as interviewee. Conduct interviews to earn more!");
            }
        }

        // 3. Add to queue
        var entry = await queueService.EnqueueAsync(
            command.UserId,
            command.Role,
            command.Difficulty,
            command.EnableVideo,
            token);

        // 4. Get ahead count
        var aheadCount = await queueService.GetAheadCountAsync(command.UserId, token);

        return new MatchQueueDto(
            entry.QueueId,
            QueueStatus.Waiting,
            aheadCount,
            entry.EnteredAt);
    }
}