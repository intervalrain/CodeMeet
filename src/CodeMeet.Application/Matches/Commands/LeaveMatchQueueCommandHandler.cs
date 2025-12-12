using CodeMeet.Ddd.Application.Cqrs.Authorization;
using CodeMeet.Ddd.Application.Cqrs.Models;
using ErrorOr;

namespace CodeMeet.Application.Matches.Commands;

public record LeaveMatchQueueCommand(Guid UserId) : IAuthorizeableCommand<ErrorOr<Unit>>;

public class LeaveMatchQueueCommandHandler(IMatchQueueService queueService)
    : ICommandHandler<LeaveMatchQueueCommand, ErrorOr<Unit>>
{
    public async Task<ErrorOr<Unit>> HandleAsync(
        LeaveMatchQueueCommand command,
        CancellationToken token = default)
    {
        var removed = await queueService.DequeueAsync(command.UserId, token);

        if (!removed)
        {
            return Error.NotFound(description: "You are not in the match queue");
        }

        return Unit.Value;
    }
}