using Asp.Versioning;

using CodeMeet.Api.Models.Users;
using CodeMeet.Application.Users.Commands;
using CodeMeet.Application.Users.Queries;
using CodeMeet.Ddd.Application.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeMeet.Api.Controllers.V1;

[ApiVersion("1.0")]
public class UserController(IDispatcher dispatcher) : ApiController
{
    private readonly IDispatcher _dispatcher = dispatcher;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllUsers()
    {
        var query = new GetUsersQuery();
        var result = await _dispatcher.QueryAsync(query);
        return Result(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var query = new GetUserQuery(id);
        var result = await _dispatcher.QueryAsync(query);
        return Result(result);
    }

    [HttpGet("me")]
    public Task<IActionResult> Me()
    {
        return Task.FromResult<IActionResult>(Ok());
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUserPassword(UpdateUserDto input)
    {
        var command = new UpdateUserCommand(input.Id, input.Password, input.NewPassword);
        var result = await _dispatcher.SendAsync(command);
        return Result(result);
    }
}