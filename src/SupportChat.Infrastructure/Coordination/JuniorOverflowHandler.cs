using SupportChat.Application.Interfaces.Coordination;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.Agents;

namespace SupportChat.Infrastructure.Coordination;

internal class JuniorOverflowHandler : IOverflowHandler
{
	private readonly IAgentRepository _agentRepo;
	private readonly IOfficeHoursProvider _officeHoursProvider;

	public JuniorOverflowHandler(IAgentRepository agentRepo, IOfficeHoursProvider officeHoursProvider)
	{
		_agentRepo = agentRepo;
		_officeHoursProvider = officeHoursProvider;
	}

	public async Task<IReadOnlyCollection<Agent>> GetOverflowAgentsAsync()
	{
		var all = await _agentRepo.GetBySeniorityAsync(Seniority.Junior);
		return all;
	}

	public bool ShouldOverflow(int processedSessions, int normalCapacity)
	{
		return processedSessions >= normalCapacity && 
			   _officeHoursProvider.IsOfficeHours(DateTime.UtcNow);
	}
}
