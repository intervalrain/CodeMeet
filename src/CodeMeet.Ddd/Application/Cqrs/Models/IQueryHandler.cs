namespace CodeMeet.Ddd.Application.Cqrs.Models;

public interface IQueryHandler<in TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query, CancellationToken token = default);
}