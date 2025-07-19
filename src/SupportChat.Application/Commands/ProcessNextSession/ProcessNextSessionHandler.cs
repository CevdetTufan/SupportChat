using SupportChat.Application.Commands.AssignSession;
using SupportChat.Application.Interfaces.Commands;
using SupportChat.Application.Interfaces.Coordination;
using SupportChat.Application.Interfaces.Repositories;

namespace SupportChat.Application.Commands.ProcessNextSession;

public class ProcessNextSessionHandler : ICommandHandler<ProcessNextSessionCommand>
{
	private readonly IChatSessionRepository _sessionRepo;
	private readonly IAgentRepository _agentRepo;
	private readonly ISessionAssignmentStrategy _strategy;
	private readonly IOverflowHandler _overflow;
	private readonly ICommandHandler<AssignSessionCommand> _assignHandler;

	public ProcessNextSessionHandler(
		IChatSessionRepository sessionRepo,
		IAgentRepository agentRepo,
		ISessionAssignmentStrategy strategy,
		IOverflowHandler overflow,
		ICommandHandler<AssignSessionCommand> assignHandler)
	{
		_sessionRepo = sessionRepo;
		_agentRepo = agentRepo;
		_strategy = strategy;
		_overflow = overflow;
		_assignHandler = assignHandler;
	}

	public async Task HandleAsync(ProcessNextSessionCommand command)
	{
		var session = await _sessionRepo.GetNextUnassignedAsync();
		if (session is null)
			return;

		var activeAgents = await _agentRepo.GetActiveAgentsAsync();
		var normalCapacity = activeAgents.Sum(a => a.MaxConcurrentChats);

		var total = await _sessionRepo.CountAllAsync();
		var queued = await _sessionRepo.CountQueuedAsync();
		var processed = total - queued;
		var pool = _overflow.ShouldOverflow(processed, normalCapacity)
						  ? await _overflow.GetOverflowAgentsAsync()
						  : activeAgents;

		var chosen = await _strategy.SelectAgentAsync(session, pool);
		if (chosen is null)
			return;

		await _assignHandler.HandleAsync(
			new AssignSessionCommand(session.Id, chosen.Id));
	}
}