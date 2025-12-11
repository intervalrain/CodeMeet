namespace CodeMeet.Ddd.Application.Cqrs.Internal;

public sealed class CommandHandlerInvoker(Type commandType, Type handlerType) : HandlerInvoker
{
    private readonly Type _commandType = commandType;
    private readonly Type _handlerType = handlerType;

    public override async Task<TResult> InvokeAsync<TResult>(IServiceProvider serviceProvider, object request, CancellationToken token)
    {
        var handler = serviceProvider.GetService(_handlerType)
            ?? throw new InvalidOperationException($"No handler registered for command type {_commandType.Name}");

        var behaviors = GetPipelineBehaviors<TResult>(serviceProvider, _commandType);

        async Task<TResult> HandleCore()
        {
            var method = _handlerType.GetMethod("HandleAsync")!;
            var task = (Task<TResult>)method.Invoke(handler, [request, token])!;
            return await task;
        }

        return await ExecutePipeline(behaviors, request, HandleCore, token);
    }
}