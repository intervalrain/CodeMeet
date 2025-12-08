namespace CodeMeet.Ddd.Application.Cqrs.Authorization;

public interface ICurrentUserProvider
{
    public CurrentUser CurrentUser { get;}
}