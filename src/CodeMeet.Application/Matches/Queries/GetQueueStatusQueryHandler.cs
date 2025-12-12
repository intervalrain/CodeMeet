using CodeMeet.Ddd.Application.Cqrs.Authorization;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Domain.Matches.Enums;
using ErrorOr;

namespace CodeMeet.Application.Matches.Queries;

public record GetQueueStatusQuery(Guid UserId) : IAuthorizeableQuery<ErrorOr<QueueStatusDto>>;

public record QueueStatusDto(
    Guid? QueueId,
    bool IsInQueue,
    QueueStatus? Status,
    int AheadCount,
    DateTime? EnteredAt);

public class GetQueueStatusQueryHandler(IMatchQueueService queueService)
    : IQueryHandler<GetQueueStatusQuery, ErrorOr<QueueStatusDto>>
{
    public async Task<ErrorOr<QueueStatusDto>> HandleAsync(
        GetQueueStatusQuery query,
        CancellationToken token = default)
    {
        var entry = await queueService.GetEntryAsync(query.UserId, token);

        if (entry is null)
        {
            return new QueueStatusDto(
                QueueId: null,
                IsInQueue: false,
                Status: null,
                AheadCount: 0,
                EnteredAt: null);
        }

        var aheadCount = await queueService.GetAheadCountAsync(query.UserId, token);

        return new QueueStatusDto(
            QueueId: entry.QueueId,
            IsInQueue: true,
            Status: QueueStatus.Waiting,
            AheadCount: aheadCount,
            EnteredAt: entry.EnteredAt);
    }
}
