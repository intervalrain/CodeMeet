using CodeMeet.Application.Gamification;
using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Authorization;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users.Entities;
using ErrorOr;

namespace CodeMeet.Application.Users.Queries;

public record GetMeQuery(Guid UserId) : IAuthorizeableQuery<ErrorOr<MeDto>>;

public class GetMeQueryHandler(
    IRepository<User> userRepository,
    IGamificationService opportunityService) : IQueryHandler<GetMeQuery, ErrorOr<MeDto>>
{
    public async Task<ErrorOr<MeDto>> HandleAsync(GetMeQuery query, CancellationToken token = default)
    {
        var user = await userRepository.FindAsync(query.UserId, token);
        if (user is null)
        {
            return Error.NotFound(description: "User not found");
        }

        var opportunities = await opportunityService.GetOpportunitiesAsync(query.UserId, token);

        return MeDto.FromEntity(user, opportunities);
    }
}