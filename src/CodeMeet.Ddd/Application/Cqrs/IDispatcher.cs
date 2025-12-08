using CodeMeet.Ddd.Application.Cqrs.Models;

namespace CodeMeet.Ddd.Application.Cqrs;

/// <summary>
/// Dispatches commands and queries to their respective handlers.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Sends a command to its handler.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the command.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>The result of the command.</returns>
    Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken token = default);
    
    /// <summary>
    /// Sends a query to its handler.
    /// </summary>
    /// <typeparam name="TResult">The type of result returned by the query.</typeparam>
    /// <param name="command">The query to send.</param>
    /// <param name="token">Cancellation token.</param>
    /// <returns>The result of the query.</returns>
    Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken token = default);
}