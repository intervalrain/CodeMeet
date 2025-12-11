using System.Diagnostics;
using CodeMeet.Ddd.Application.Cqrs.Audit;
using CodeMeet.Ddd.Application.Cqrs.Behaviors.Options;
using CodeMeet.Ddd.Application.Cqrs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeMeet.Ddd.Application.Cqrs.Behaviors;

/// <summary>
/// Pipeline behavior that adds audit information (trace ID, correlation ID) to the request context
/// and logs audit trail for commands.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResult">The type of result.</typeparam>
public sealed class AuditBehavior<TRequest, TResult>(
    IAuditContext auditContext,
    ILogger<AuditBehavior<TRequest, TResult>> logger,
    IOptions<BehaviorOptions> options) : IPipelineBehavior<TRequest, TResult>
    where TRequest : notnull
{
    private readonly IAuditContext _context = auditContext;
    private readonly ILogger<AuditBehavior<TRequest, TResult>> _logger = logger;
    private readonly AuditBehaviorOptions _options = options.Value.Audit;

    public async Task<TResult> HandleAsync(TRequest request, Func<Task<TResult>> next, CancellationToken token = default)
    {
        if (!_options.Enabled || !_options.Scope.Matches<TRequest>())
        {
            return await next();
        }

        var requestName = typeof(TRequest).Name;
        var traceId = _context.TraceId;
        var correlationId = _context.CorrelationId;
        var userId = _context.UserId;

        Activity.Current?.SetTag("trace.id", traceId);
        Activity.Current?.SetTag("correlation.id", correlationId);
        Activity.Current?.SetTag("user.id", userId);

        using var scope = _logger.BeginScope(new Dictionary<string, object?>
        {
            { "TraceId", traceId },
            { "CorrelationId", correlationId },
            { "UserId", userId },
            { "RequestType", requestName },
        });

        _logger.LogDebug(
            "Processing {RequestName} | TraceId: {TraceId} | CorrelationId: {CorrelationId} | UserId: {UserId}",
            requestName, traceId, correlationId, userId);

        try
        {
            var result = await next();

            _logger.LogInformation(
                "Audit: {RequestName} completed | TraceId: {TraceId} | UserId: {UserId}",
                requestName, traceId, userId);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Audit: {RequestType} failed | TraceId: {TraceId} | UserId: {UserId} | Error: {ErrorMessage}",
                requestName, traceId, userId, ex.Message);
            throw;
        }
    }
}