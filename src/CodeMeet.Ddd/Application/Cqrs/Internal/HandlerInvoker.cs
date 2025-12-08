using CodeMeet.Ddd.Application.Cqrs.Models;

namespace CodeMeet.Ddd.Application.Cqrs.Internal;

public abstract class HandlerInvoker
{
    public abstract Task<TResult> InvokeAsync<TResult>(IServiceProvider serviceProvider, object request, CancellationToken token);

    protected static IReadOnlyList<object> GetPipelineBehaviors<TResult>(IServiceProvider serviceProvider, Type requestType)
    {
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResult));
        var behaviors = (IEnumerable<object>?)serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(behaviorType));
        return behaviors?.ToList() ?? [];
    }

    protected static async Task<TResult> ExecutePipeline<TResult>(
        IReadOnlyList<object> behaviors,
        object request,
        Func<Task<TResult>> handler,
        CancellationToken token)
    {
        if (behaviors.Count == 0) return await handler();

        var index = 0;

        async Task<TResult> Next()
        {
            if (index >= behaviors.Count) return await handler();

            var behavior = behaviors[index++];
            var method = behavior.GetType().GetMethod("HandleAsync")!;
            var task = (Task<TResult>)method.Invoke(behavior, [request, (Func<Task<TResult>>)Next, token])!;

            return await task;
        }

        return await Next();
    }
}