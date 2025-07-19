using SupportChat.Application.Interfaces.Queries;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.ChatSessions;

namespace SupportChat.Application.Queries.ChatSessions;

internal class GetNextUnassignedSessionHandler : IQueryHandler<GetNextUnassignedSessionQuery, ChatSession?>
{
	private readonly IChatSessionRepository _repo;

	public GetNextUnassignedSessionHandler(IChatSessionRepository repo)
	{
		_repo = repo;
	}
	public async Task<ChatSession?> HandleAsync(GetNextUnassignedSessionQuery query)
	{
		return await _repo.GetNextUnassignedAsync();
	}
}
