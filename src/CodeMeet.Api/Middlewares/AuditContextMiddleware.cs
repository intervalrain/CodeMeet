using System.Security.Claims;
using CodeMeet.Ddd.Application.Cqrs.Audit;

namespace CodeMeet.Api.Middlewares;

public class AuditContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IAuditContext auditContext)
    {
        var userId = context.User.FindFirstValue("id");

        if (!string.IsNullOrEmpty(userId))
        {
            auditContext.UserId = userId;
        }

        if (context.Request.Headers.TryGetValue("X-Correlation-Id", out var correlationId))
        {
            auditContext.CorrelationId = correlationId;
        }

        await next(context);
    }   
}