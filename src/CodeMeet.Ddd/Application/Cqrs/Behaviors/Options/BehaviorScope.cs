namespace CodeMeet.Ddd.Application.Cqrs.Behaviors.Options;

/// <summary>
/// Defines which request types a behavior should apply to.
/// </summary>
[Flags]
public enum BehaviorScope
{
    None = 0,
    Command = 1,
    Query = 2,
    All = Command | Query
}
