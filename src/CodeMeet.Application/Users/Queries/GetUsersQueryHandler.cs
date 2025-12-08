
using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users;

using ErrorOr;

namespace CodeMeet.Application.Users.Queries;

public record GetUsersQuery : IQuery<ErrorOr<IEnumerable<UserDto>>>;

public class GetUsersQueryHandler : IQueryHandler<GetUsersQuery, ErrorOr<IEnumerable<UserDto>>>
{
    private readonly IRepository<User> _repository;

    public GetUsersQueryHandler(IRepository<User> repository)
    {
        _repository = repository;
    }

    public async Task<ErrorOr<IEnumerable<UserDto>>> HandleAsync(GetUsersQuery query, CancellationToken token = default)
    {
        var users = await _repository.GetListAsync();

        return users.Select(user => new UserDto(user.Id, user.Username)).ToErrorOr();
    }
}