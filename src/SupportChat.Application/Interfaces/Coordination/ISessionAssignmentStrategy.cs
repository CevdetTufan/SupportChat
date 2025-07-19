using SupportChat.Domain.Agents;
using SupportChat.Domain.ChatSessions;

namespace SupportChat.Application.Interfaces.Coordination;

public interface ISessionAssignmentStrategy
{
	Task<Agent?> SelectAgentAsync(
			ChatSession session,
			IReadOnlyCollection<Agent> pool);
}
