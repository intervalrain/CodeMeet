namespace CodeMeet.Ddd.Application.Cqrs.Behaviors.Options;

/// <summary>
/// Root configuration for all pipeline behaviors.
/// </summary>
public class BehaviorOptions
{
    public const string SectionName = "BehaviorOptions";

    public AuditBehaviorOptions Audit { get; set; } = new();
    public AuthorizationBehaviorOptions Authorization { get; set; } = new();
    public LoggingBehaviorOptions Logging { get; set; } = new();
    public TransactionBehaviorOptions Transaction { get; set; } = new();
    public ValidationBehaviorOptions Validation { get; set; } = new();
}
