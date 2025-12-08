using System.Collections.Concurrent;

using CodeMeet.Ddd.Application.Cqrs.Internal;
using CodeMeet.Ddd.Application.Cqrs.Models;

namespace CodeMeet.Ddd.Application.Cqrs;

public sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private static readonly ConcurrentDictionary<Type, HandlerInvoker> _commandHandlerCache = [];
    private static readonly ConcurrentDictionary<Type, HandlerInvoker> _queryHandlerCache = [];

    public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var commandType = command.GetType();
        var invoker = _commandHandlerCache.GetOrAdd(commandType, CreateCommandInvoker<TResult>);
        
        return await invoker.InvokeAsync<TResult>(_serviceProvider, command, token);
    }

    public async Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var queryType = query.GetType();
        var invoker = _queryHandlerCache.GetOrAdd(queryType, CreateQueryInvoker<TResult>);

        return await invoker.InvokeAsync<TResult>(_serviceProvider, query, token);
    }

    private static HandlerInvoker CreateCommandInvoker<TResult>(Type commandType)
    {
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType, typeof(TResult));
        return new CommandHandlerInvoker(commandType, handlerType);
    }

    private static HandlerInvoker CreateQueryInvoker<TResult>(Type queryType)
    {
        var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));
        return new QueryHandlerInvoker(queryType, handlerType);
    }
}