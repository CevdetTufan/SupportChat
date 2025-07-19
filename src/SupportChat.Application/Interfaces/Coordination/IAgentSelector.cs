using SupportChat.Domain.Agents;
using SupportChat.Domain.ChatSessions;

namespace SupportChat.Application.Interfaces.Coordination;

public interface IAgentSelector
{
	Task<Agent?> SelectAgentAsync(ChatSession session, int processedSessions, int normalCapacity);
}
