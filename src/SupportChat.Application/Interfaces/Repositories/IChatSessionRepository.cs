using SupportChat.Domain.ChatSessions;

namespace SupportChat.Application.Interfaces.Repositories;

public interface IChatSessionRepository: IRepository<ChatSession>
{
	Task<ChatSession?> GetNextUnassignedAsync(CancellationToken token = default);
	Task<int> CountQueuedAsync(CancellationToken token = default);
	Task<int> CountAllAsync(CancellationToken token = default);
}
