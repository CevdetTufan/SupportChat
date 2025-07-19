namespace SupportChat.Application.Interfaces.Queries;

public interface IQueryHandler<TQuery, TResult>
{
	Task<TResult> HandleAsync(TQuery query);
}
