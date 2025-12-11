using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users.Entities;
using ErrorOr;

namespace CodeMeet.Application.Users.Queries;

public record GetUserQuery(Guid Id) : IQuery<ErrorOr<UserDto>>;

public class GetUserQueryHandler(IRepository<User> repository) : IQueryHandler<GetUserQuery, ErrorOr<UserDto>>
{
    public async Task<ErrorOr<UserDto>> HandleAsync(GetUserQuery query, CancellationToken token = default)
    {
        var user = await repository.FindAsync(query.Id, token);

        return user is null ? Error.NotFound(description: "User not found") : UserDto.FromEntity(user);
    }
}