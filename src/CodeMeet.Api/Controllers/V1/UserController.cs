using Asp.Versioning;
using CodeMeet.Api.Contracts.Users;
using CodeMeet.Application.Users.Commands;
using CodeMeet.Application.Users.Queries;
using CodeMeet.Ddd.Application.Cqrs;
using CodeMeet.Ddd.Application.Cqrs.Audit;
using CodeMeet.Ddd.Application.Cqrs.Authorization;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeMeet.Api.Controllers.V1;

[ApiVersion("1.0")]
public class UserController(IDispatcher dispatcher, IAuditContext auditContext, ICurrentUserProvider currentUserProvider) : ApiController
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetUsersQuery(page, pageSize);
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
    public async Task<IActionResult> Register(CreateUserRequest request)
    {
        var command = new CreateUserCommand(request.Username, request.Password, request.Email, request.DisplayName);
        var result = await dispatcher.SendAsync(command);
        return result.Match(
            v => CreatedAtAction(
                actionName: nameof(GetUser),
                controllerName: "User",
                routeValues: new { id = v.User.Id },
                value: v),
            Problem);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser(UpdateUserRequest request)
    {
        var userId = currentUserProvider.CurrentUser.Id;
        var command = new UpdateUserCommand(userId, request.Password, request.NewPassword, request.DisplayName);
        var result = await dispatcher.SendAsync(command);
        return NoContent(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var command = new DeleteUserCommand(id);
        var result = await dispatcher.SendAsync(command);
        return NoContent(result);
    }

    [HttpPut("me/preferences")]
    public async Task<IActionResult> UpdateUserPreferences(UpdateUserPreferencesRequest request)
    {
        var userId = currentUserProvider.CurrentUser.Id;
        var command = new UpdateUserPreferencesCommand(
            userId,
            request.Languages,
            request.Difficulty,
            request.EnableVideo);
        
        var result = await dispatcher.SendAsync(command);
        return Result(result);
    }
}