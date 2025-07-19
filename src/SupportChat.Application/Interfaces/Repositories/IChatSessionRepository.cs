using SupportChat.Domain.ChatSessions;

namespace SupportChat.Application.Interfaces.Repositories;

public interface IChatSessionRepository: IRepository<ChatSession>
{
	Task<ChatSession?> GetNextUnassignedAsync();
	Task<int> CountQueuedAsync();
}
