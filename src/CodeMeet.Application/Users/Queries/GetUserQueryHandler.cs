using CodeMeet.Application.Users.Dtos;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users.Entities;
using ErrorOr;

namespace CodeMeet.Application.Users.Queries;

public record GetUserQuery(Guid Id) : IQuery<ErrorOr<UserDto>>;

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, ErrorOr<UserDto>>
{
    private readonly IRepository<User> _repository;

    public GetUserQueryHandler(IRepository<User> repository)
    {
        _repository = repository;
    }

    public async Task<ErrorOr<UserDto>> HandleAsync(GetUserQuery query, CancellationToken token = default)
    {
        var user = await _repository.FindAsync(query.Id, token);

        return user is null ? Error.NotFound(description: "User not found") : new UserDto(user.Id, user.Username).ToErrorOr();
    }
}