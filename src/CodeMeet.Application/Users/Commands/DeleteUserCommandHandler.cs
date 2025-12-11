using CodeMeet.Application.Common.Security;
using CodeMeet.Ddd.Application.Cqrs.Authorization;
using CodeMeet.Ddd.Application.Cqrs.Models;
using CodeMeet.Ddd.Infrastructure;
using CodeMeet.Domain.Users.Entities;
using ErrorOr;

namespace CodeMeet.Application.Users.Commands;

[Authorize(Policies = Policy.AdminOnly)]
public record DeleteUserCommand(Guid UserId) : IAuthorizeableCommand<ErrorOr<Deleted>>;

public class DeleteUserCommandHandler(IRepository<User> repository) : ICommandHandler<DeleteUserCommand, ErrorOr<Deleted>>
{
    public async Task<ErrorOr<Deleted>> HandleAsync(DeleteUserCommand command, CancellationToken token = default)
    {
        var user = await repository.FindAsync(command.UserId, token);
        if (user is null)
        {
            return Error.NotFound(description: "User not found");
        }

        if (user.Roles.Contains(Role.Admin))
        {
            return Error.Forbidden(description: "Admin users cannot be deleted");
        }

        await repository.DeleteAsync(user, token);

        return Result.Deleted;
    }
}