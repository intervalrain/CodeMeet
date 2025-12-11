namespace CodeMeet.Ddd.Application.Cqrs.Behaviors.Options;

/// <summary>
/// Configuration for logging behavior.
/// </summary>
public class LoggingBehaviorOptions
{
    /// <summary>
    /// Whether the logging behavior is enabled. Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Which request types to log. Default: All.
    /// </summary>
    public BehaviorScope Scope { get; set; } = BehaviorScope.All;

    /// <summary>
    /// Threshold in milliseconds for slow request warnings. Default: 500ms.
    /// </summary>
    public int SlowRequestThresholdMs { get; set; } = 500;
}
