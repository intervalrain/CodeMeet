using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users.Entities;
using ErrorOr;

namespace CodeMeet.Application.Users.Queries;

public record GetUsersQuery : IQuery<ErrorOr<IEnumerable<UserDto>>>;

public class GetUsersQueryHandler(IRepository<User> repository) : IQueryHandler<GetUsersQuery, ErrorOr<IEnumerable<UserDto>>>
{
    public async Task<ErrorOr<IEnumerable<UserDto>>> HandleAsync(GetUsersQuery query, CancellationToken token = default)
    {
        var users = await repository.GetListAsync(token);

        return users.Select(UserDto.FromEntity).ToErrorOr();
    }
}