using CodeMeet.Application.Users.Queries;
using CodeMeet.Ddd.Application.Cqrs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeMeet.Api.Controllers;

[AllowAnonymous]
public class UserController(IDispatcher dispatcher) : ApiController
{
    private readonly IDispatcher _dispatcher = dispatcher;

    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        var query = new GetUsersQuery();

        var result = await _dispatcher.QueryAsync(query);
        return result.Match(Ok, Problem);
    }
}