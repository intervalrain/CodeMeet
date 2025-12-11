using Asp.Versioning;
using CodeMeet.Api.Models.Auth;
using CodeMeet.Application.Auth.Commands;
using CodeMeet.Ddd.Application.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeMeet.Api.Controllers.V1;

[ApiVersion("1.0")]
[AllowAnonymous]
public class AuthController(IDispatcher dispatcher) : ApiController
{
    private readonly IDispatcher _dispatcher = dispatcher;

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto input)
    {
        var command = new RegisterUserCommand(input.Username, input.Password, input.Email);
        var result = await _dispatcher.SendAsync(command);
        return result.Match(
            v => CreatedAtAction(
                actionName: nameof(UserController.GetUser),
                controllerName: "User",
                routeValues: new { id = v.User.Id },
                value: v),
            Problem);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto input)
    {
        var command = new LoginUserCommand(input.Username, input.Password);
        var result = await _dispatcher.SendAsync(command);
        return Result(result);
    }
}