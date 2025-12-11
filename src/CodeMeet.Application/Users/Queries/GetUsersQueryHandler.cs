using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users.Entities;
using ErrorOr;

namespace CodeMeet.Application.Users.Queries;

public record GetUsersQuery(int PageNumber = 1, int PageSize = 20) : PaginationQuery<ErrorOr<PaginationResult<UserDto>>>(PageNumber, PageSize);

public class GetUsersQueryHandler(IRepository<User> repository) : IQueryHandler<GetUsersQuery, ErrorOr<PaginationResult<UserDto>>>
{
    public async Task<ErrorOr<PaginationResult<UserDto>>> HandleAsync(GetUsersQuery query, CancellationToken token = default)
    {
        var users = await repository.GetPagedListAsync(query, token);

        return users.Map(UserDto.FromEntity).ToErrorOr();
    }
}