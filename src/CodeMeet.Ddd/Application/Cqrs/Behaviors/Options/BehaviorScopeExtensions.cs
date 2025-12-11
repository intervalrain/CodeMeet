using CodeMeet.Ddd.Application.Cqrs.Models;

namespace CodeMeet.Ddd.Application.Cqrs.Behaviors.Options;

public static class BehaviorScopeExtensions
{
    /// <summary>
    /// Checks if the request type matches the specified scope.
    /// </summary>
    public static bool Matches<TRequest>(this BehaviorScope scope)
    {
        if (scope == BehaviorScope.None)
            return false;

        if (scope == BehaviorScope.All)
            return true;

        var isCommand = IsCommand<TRequest>();
        var isQuery = IsQuery<TRequest>();

        if (scope.HasFlag(BehaviorScope.Command) && isCommand)
            return true;

        if (scope.HasFlag(BehaviorScope.Query) && isQuery)
            return true;

        return false;
    }

    private static bool IsCommand<TRequest>()
    {
        return typeof(TRequest).GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommand<>));
    }

    private static bool IsQuery<TRequest>()
    {
        return typeof(TRequest).GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>));
    }
}
