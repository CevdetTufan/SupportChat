using SupportChat.Application.Interfaces.Coordination;
using SupportChat.Application.Interfaces.Persistence;
using SupportChat.Application.Interfaces.Repositories;

namespace SupportChat.Infrastructure.Coordination;

internal class ChatCoordinatorEngine : IChatCoordinatorEngine
{
	private readonly IChatSessionRepository _sessionRepo;
	private readonly IAgentRepository _agentRepo;
	private readonly IAgentSelector _agentSelector;
	private readonly IUnitOfWork _uow;

	public ChatCoordinatorEngine(
		IChatSessionRepository sessionRepo,
		IAgentRepository agentRepo,
		IAgentSelector agentSelector,
		IUnitOfWork uow)
	{
		_sessionRepo = sessionRepo;
		_agentRepo = agentRepo;
		_agentSelector= agentSelector;
		_uow = uow;
	}


	public async Task ProcessNextSessionAsync()
	{
		var session = await _sessionRepo.GetNextUnassignedAsync();
		if (session is null)
			return;

		var agents = await _agentRepo.GetActiveAgentsAsync();

		var normalCapacity = agents.Sum(a => a.MaxConcurrentChats);
		var queueLength = await _sessionRepo.CountQueuedAsync();

		var chosen = await _agentSelector.SelectAgentAsync(
		   session, queueLength, normalCapacity);

		if (chosen is null)
			return; 

		session.AssignToAgent(chosen.Id);
		chosen.AssignChat(session.Id);

		_sessionRepo.Update(session);
		_agentRepo.Update(chosen);
		await _uow.SaveChangesAsync();
	}
}
