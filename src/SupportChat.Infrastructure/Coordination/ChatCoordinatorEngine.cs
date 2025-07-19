using SupportChat.Application.Interfaces.Coordination;
using SupportChat.Application.Interfaces.Persistence;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.Agents;

namespace SupportChat.Infrastructure.Coordination;

internal class ChatCoordinatorEngine : IChatCoordinatorEngine
{
	private readonly IChatSessionRepository _sessionRepo;
	private readonly IAgentRepository _agentRepo;
	private readonly ISessionAssignmentStrategy _strategy;
	private readonly IUnitOfWork _uow;

	public ChatCoordinatorEngine(
		IChatSessionRepository sessionRepo,
		IAgentRepository agentRepo,
		ISessionAssignmentStrategy strategy,
		IUnitOfWork uow)
	{
		_sessionRepo = sessionRepo;
		_agentRepo = agentRepo;
		_strategy = strategy;
		_uow = uow;
	}


	public async Task ProcessNextSessionAsync()
	{
		var session = await _sessionRepo.GetNextUnassignedAsync();
		if (session is null) return;

		var normalAgents = await _agentRepo.GetNormalAgentsAsync();
		var normalCapacity = normalAgents.Sum(a => a.MaxConcurrentChats);

		var total = await _sessionRepo.CountAllAsync();
		var queued = await _sessionRepo.CountQueuedAsync();
		var processed = total - queued;


		IReadOnlyCollection<Agent> pool = processed < normalCapacity
			? normalAgents.ToList()
			: (await _agentRepo.GetOverflowAgentsAsync()).ToList(); 

		var chosen = await _strategy.SelectAgentAsync(session, pool);
		if (chosen is null) return;

		session.AssignToAgent(chosen.Id);
		chosen.AssignChat(session.Id);

		_sessionRepo.Update(session);
		_agentRepo.Update(chosen);
		await _uow.SaveChangesAsync();
	}
}
