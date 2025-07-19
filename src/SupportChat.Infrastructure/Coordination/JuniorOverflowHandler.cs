using SupportChat.Application.Interfaces.Coordination;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.Agents;

namespace SupportChat.Infrastructure.Coordination;

internal class JuniorOverflowHandler : IOverflowHandler
{
	private readonly IAgentRepository _agentRepo;

	public JuniorOverflowHandler(IAgentRepository agentRepo)
	{
		_agentRepo = agentRepo;
	}

	public async Task<IReadOnlyCollection<Agent>> GetOverflowAgentsAsync()
	{
		var all = await _agentRepo.GetBySeniorityAsync(Seniority.Junior);
		return all;
	}

	public bool ShouldOverflow(int currentQueueLength, int normalCapacity)
	{
		return currentQueueLength >= normalCapacity;
	}
}
