using SupportChat.Application.Interfaces.Coordination;
using SupportChat.Domain.Agents;
using SupportChat.Domain.ChatSessions;

namespace SupportChat.Infrastructure.Coordination;

internal class SeniorityBasedAssignment : ISessionAssignmentStrategy
{
	public Task<Agent?> SelectAgentAsync(ChatSession session, IReadOnlyCollection<Agent> pool)
	{
		var candidate = pool
			.Where(a => a.CanAcceptChat())
			.OrderBy(a => a.ActiveChatIds.Count)
			.FirstOrDefault();

		return Task.FromResult(candidate);
	}
}
