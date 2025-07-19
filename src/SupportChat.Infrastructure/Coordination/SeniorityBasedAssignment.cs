using SupportChat.Application.Interfaces.Coordination;
using SupportChat.Domain.Agents;
using SupportChat.Domain.ChatSessions;

namespace SupportChat.Infrastructure.Coordination;

internal class SeniorityBasedAssignment : ISessionAssignmentStrategy
{
	public Task<Agent?> SelectAgentAsync(ChatSession session, IReadOnlyCollection<Agent> availableAgents)
	{
		var groups = availableAgents
				.OrderBy(a => a.Seniority)
				.GroupBy(a => a.Seniority);

		foreach (var bucket in groups)
		{
			var candidates = bucket
				.Where(a => a.CanAcceptChat());

			var pick = candidates
				.OrderBy(a => a.ActiveChatIds.Count)
				.FirstOrDefault();

			if (pick != null)
				return Task.FromResult<Agent?>(pick);
		}

		return Task.FromResult<Agent?>(null);
	}
}
