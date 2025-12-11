namespace CodeMeet.Ddd.Application.Cqrs.Behaviors.Options;

/// <summary>
/// Configuration for validation behavior.
/// </summary>
public class ValidationBehaviorOptions
{
    /// <summary>
    /// Whether the validation behavior is enabled. Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Which request types to validate. Default: All.
    /// </summary>
    public BehaviorScope Scope { get; set; } = BehaviorScope.All;
}
