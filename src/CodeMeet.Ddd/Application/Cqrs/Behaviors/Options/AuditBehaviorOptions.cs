namespace CodeMeet.Ddd.Application.Cqrs.Behaviors.Options;

/// <summary>
/// Configuration for audit behavior.
/// </summary>
public class AuditBehaviorOptions
{
    /// <summary>
    /// Whether the audit behavior is enabled. Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Which request types to audit. Default: Command.
    /// </summary>
    public BehaviorScope Scope { get; set; } = BehaviorScope.Command;

    /// <summary>
    /// Whether to include request payload in audit logs. Default: false.
    /// </summary>
    public bool IncludeRequestPayload { get; set; } = false;

    /// <summary>
    /// Whether to include response in audit logs. Default: false.
    /// </summary>
    public bool IncludeResponse { get; set; } = false;
}