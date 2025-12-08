namespace CodeMeet.Ddd.Application.Cqrs.Authorization;

/// <summary>
/// Marks a request as requiring authorization.
/// Can be applied multiple times to specify multiple authorization conditions.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class AuthorizeAttribute : Attribute
{
    /// <summary>
    /// Comma-separated list of required permissions.
    /// User must have ALL specified permissions.
    /// </summary>
    public string? Permissions { get; set; }
    
    /// <summary>
    /// Comma-separated list of required roles.
    /// User must have at least ONE of the specified roles.
    /// </summary>
    public string? Roles { get; set; }

    /// <summary>
    /// Comma-separated list of policy names to evaluate.
    /// All policies must pass.
    /// </summary>
    public string? Policies { get; set; }
}