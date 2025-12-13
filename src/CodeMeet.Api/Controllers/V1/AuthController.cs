using Asp.Versioning;
using CodeMeet.Api.Contracts.Auth;
using CodeMeet.Application.Auth.Commands;
using CodeMeet.Ddd.Application.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeMeet.Api.Controllers.V1;

[ApiVersion("1.0")]
[AllowAnonymous]
public class AuthController(IDispatcher dispatcher) : ApiController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest input)
    {
        var command = new LoginUserCommand(input.Username, input.Password);
        var result = await dispatcher.SendAsync(command);
        return Result(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequest input)
    {
        var command = new RevokeTokenCommand(input.RefreshToken);
        var result = await dispatcher.SendAsync(command);
        return NoContent(result);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest input)
    {
        var command = new RefreshTokenCommand(input.RefreshToken);
        var result = await dispatcher.SendAsync(command);
        return Result(result);
    }
}