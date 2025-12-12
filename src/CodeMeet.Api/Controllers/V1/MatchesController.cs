using Asp.Versioning;
using CodeMeet.Api.Contracts.Matches;
using CodeMeet.Application.Matches.Commands;
using CodeMeet.Application.Matches.Queries;
using CodeMeet.Ddd.Application.Cqrs;
using CodeMeet.Ddd.Application.Cqrs.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodeMeet.Api.Controllers.V1;

[ApiVersion("1.0")]
public class MatchesController(IDispatcher dispatcher, ICurrentUserProvider currentUserProvider) : ApiController
{
    /// <summary>
    /// Join the match queue with specified preferences.
    /// </summary>
    [HttpPost("queue")]
    public async Task<IActionResult> JoinMatchQueue([FromBody] JoinMatchQueueRequest request)
    {
        var currentUser = currentUserProvider.CurrentUser;
        var command = new JoinMatchQueueCommand(
            currentUser.Id,
            request.Role,
            request.Difficulty,
            request.EnableVideo);

        var result = await dispatcher.SendAsync(command);
        return Accepted(result);
    }

    /// <summary>
    /// Leave the match queue.
    /// </summary>
    [HttpDelete("queue")]
    public async Task<IActionResult> LeaveMatchQueue()
    {
        var currentUser = currentUserProvider.CurrentUser;
        var command = new LeaveMatchQueueCommand(currentUser.Id);
        var result = await dispatcher.SendAsync(command);
        return NoContent(result);
    }

    /// <summary>
    /// Get current queue status (position, estimated wait time).
    /// </summary>
    [HttpGet("queue/status")]
    public async Task<IActionResult> GetQueueStatus()
    {
        var currentUser = currentUserProvider.CurrentUser;
        var query = new GetQueueStatusQuery(currentUser.Id);
        var result = await dispatcher.QueryAsync(query);
        return Result(result);
    }
}

