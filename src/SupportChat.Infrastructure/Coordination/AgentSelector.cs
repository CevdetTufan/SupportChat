using SupportChat.Application.Interfaces.Coordination;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.Agents;
using SupportChat.Domain.ChatSessions;

namespace SupportChat.Infrastructure.Coordination;

internal class AgentSelector : IAgentSelector
{
	private readonly IAgentRepository _agentRepo;
	private readonly ISessionAssignmentStrategy _strategy;
	private readonly IOverflowHandler _overflow;
	public AgentSelector(
		IAgentRepository agentRepo,
		ISessionAssignmentStrategy strategy,
		IOverflowHandler overflow)
	{
		_agentRepo = agentRepo;
		_strategy = strategy;
		_overflow = overflow;
	}
	public async Task<Agent?> SelectAgentAsync(ChatSession session, int queueLength, int normalCapacity)
	{
		var allAgents = await _agentRepo.ListAllAsync();

		var activeAgents = allAgents
			.Where(a => a.Status == AgentStatus.Active)
			.ToList();

		IReadOnlyCollection<Agent> pool = _overflow
			.ShouldOverflow(queueLength, normalCapacity)
				? await _overflow.GetOverflowAgentsAsync()
				: activeAgents;

		return await _strategy.SelectAgentAsync(session, pool);
	}
}
