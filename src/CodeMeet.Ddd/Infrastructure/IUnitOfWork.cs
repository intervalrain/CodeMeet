namespace CodeMeet.Ddd.Infrastructure;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken token = default);
}