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
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto input)
    {
        var command = new LoginUserCommand(input.Username, input.Password);
        var result = await dispatcher.SendAsync(command);
        return Result(result);
    }
}