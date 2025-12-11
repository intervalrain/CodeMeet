namespace CodeMeet.Ddd.Application.Cqrs.Behaviors.Options;

/// <summary>
/// Configuration for authorization behavior.
/// </summary>
public class AuthorizationBehaviorOptions
{
    /// <summary>
    /// Whether the authorization behavior is enabled. Default: true.
    /// </summary>
    public bool Enabled { get; set; } = true;
}
