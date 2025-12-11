namespace CodeMeet.Ddd.Application.Cqrs.Behaviors.Options;

/// <summary>
/// Configuration for transaction behavior.
/// </summary>
public class TransactionBehaviorOptions
{
    /// <summary>
    /// Whether the transaction behavior is enabled. Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Which request types to wrap in transactions. Default: Command.
    /// </summary>
    public BehaviorScope Scope { get; set; } = BehaviorScope.Command;
}
