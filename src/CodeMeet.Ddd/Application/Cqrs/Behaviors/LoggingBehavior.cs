using System.Diagnostics;
using CodeMeet.Ddd.Application.Cqrs.Behaviors.Options;
using CodeMeet.Ddd.Application.Cqrs.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CodeMeet.Ddd.Application.Cqrs.Behaviors;

/// <summary>
/// Pipeline behavior that logs request processing.
/// </summary>
/// <typeparam name="TRequest">The type of request.</typeparam>
/// <typeparam name="TResult">The type of result.</typeparam>
public sealed class LoggingBehavior<TRequest, TResult>(
    ILogger<LoggingBehavior<TRequest, TResult>> logger,
    IOptions<BehaviorOptions> options) : IPipelineBehavior<TRequest, TResult>
    where TRequest : notnull
{
    private readonly ILogger<LoggingBehavior<TRequest, TResult>> _logger = logger;
    private readonly LoggingBehaviorOptions _options = options.Value.Logging;

    public async Task<TResult> HandleAsync(TRequest request, Func<Task<TResult>> next, CancellationToken ct = default)
    {
        if (!_options.Enabled || !_options.Scope.Matches<TRequest>())
        {
            return await next();
        }

        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Handling {RequestName}", requestName);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await next();
            stopwatch.Stop();

            var elapsedMs = stopwatch.ElapsedMilliseconds;
            if (elapsedMs > _options.SlowRequestThresholdMs)
            {
                _logger.LogWarning(
                    "Slow request: {RequestName} took {ElapsedMilliseconds}ms (threshold: {ThresholdMs}ms)",
                    requestName,
                    elapsedMs,
                    _options.SlowRequestThresholdMs);
            }
            else
            {
                _logger.LogInformation(
                    "Handled {RequestName} in {ElapsedMilliseconds}ms",
                    requestName,
                    elapsedMs);
            }

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            _logger.LogError(
                ex,
                "Error handling {RequestName} after {ElapsedMilliseconds}ms",
                requestName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
