
namespace CodeMeet.Ddd.Application.Cqrs.Internal;

public class QueryHandlerInvoker(Type queryType, Type handlerType) : HandlerInvoker
{
    private readonly Type _queryType = queryType;
    private readonly Type _handlerType = handlerType;

    public override async Task<TResult> InvokeAsync<TResult>(IServiceProvider serviceProvider, object request, CancellationToken token)
    {
        var handler = serviceProvider.GetService(_handlerType)
            ?? throw new InvalidOperationException($"No handler registered for query type {_queryType}");

        var behaviors = GetPipelineBehaviors<TResult>(serviceProvider, _queryType);

        async Task<TResult> HandleCore()
        {
            var method = _handlerType.GetMethod("HandleAsync")!;
            var task = (Task<TResult>)method.Invoke(handler, [request, token])!;
            return await task;
        }

        return await ExecutePipeline(behaviors, request, HandleCore, token);
    }
}