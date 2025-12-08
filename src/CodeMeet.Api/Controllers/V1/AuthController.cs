using Asp.Versioning;
using CodeMeet.Api.Models.Auth;
using CodeMeet.Application.Users.Commands;
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
        var command = new RegisterUserCommand(input.Username, input.Password);

        var result = await _dispatcher.SendAsync(command);
        return result.Match(Ok, Problem);
    }
}