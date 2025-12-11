using Asp.Versioning;
using CodeMeet.Api.Models.Users;
using CodeMeet.Application.Users.Commands;
using CodeMeet.Application.Users.Queries;
using CodeMeet.Ddd.Application.Cqrs;
using CodeMeet.Ddd.Application.Cqrs.Audit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeMeet.Api.Controllers.V1;

[ApiVersion("1.0")]
public class UserController(IDispatcher dispatcher, IAuditContext auditContext) : ApiController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllUsers()
    {
        var query = new GetUsersQuery();
        var result = await dispatcher.QueryAsync(query);
        return Result(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var query = new GetUserQuery(id);
        var result = await dispatcher.QueryAsync(query);
        return Result(result);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = Guid.Parse(auditContext.UserId!);
        var query = new GetMeQuery(userId);
        var result = await dispatcher.QueryAsync(query);
        return Result(result);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(CreateUserDto input)
    {
        var command = new CreateUserCommand(input.Username, input.Password, input.Email, input.DisplayName);
        var result = await dispatcher.SendAsync(command);
        return result.Match(
            v => CreatedAtAction(
                actionName: nameof(UserController.GetUser),
                controllerName: "User",
                routeValues: new { id = v.User.Id },
                value: v),
            Problem);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUserPassword(UpdateUserDto input)
    {
        var command = new UpdateUserCommand(input.Id, input.Password, input.NewPassword);
        var result = await dispatcher.SendAsync(command);
        return Result(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var command = new DeleteUserCommand(id);
        var result = await dispatcher.SendAsync(command);
        return Result(result);
    }
}