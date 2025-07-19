using SupportChat.Application.Commands.Agents;
using SupportChat.Application.Interfaces.Commands;
using SupportChat.Application.Interfaces.Coordination;
using SupportChat.Application.Interfaces.Queries;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Application.Queries.Agents;
using SupportChat.Application.Queries.ChatSessions;
using SupportChat.Domain.Agents;
using SupportChat.Domain.ChatSessions;

namespace SupportChat.Application.Commands.ChatSessions;

internal class ProcessNextSessionHandler : ICommandHandler<ProcessNextSessionCommand>
{
	private readonly IQueryHandler<GetNextUnassignedSessionQuery, ChatSession?> _getNextSession;
	private readonly IQueryHandler<GetActiveAgentsQuery, IReadOnlyCollection<Agent>> _getActiveAgents;
	private readonly ISessionAssignmentStrategy _strategy;
	private readonly IOverflowHandler _overflow;
	private readonly ICommandHandler<AssignSessionCommand> _assignHandler;
	private readonly IChatSessionRepository _sessionRepo;

	public ProcessNextSessionHandler(
		IQueryHandler<GetNextUnassignedSessionQuery, ChatSession?> getNextSession, 
		IQueryHandler<GetActiveAgentsQuery, IReadOnlyCollection<Agent>> getActiveAgents, 
		ISessionAssignmentStrategy strategy, 
		IOverflowHandler overflow, 
		ICommandHandler<AssignSessionCommand> assignHandler, 
		IChatSessionRepository sessionRepo)
	{
		_getNextSession = getNextSession;
		_getActiveAgents = getActiveAgents;
		_strategy = strategy;
		_overflow = overflow;
		_assignHandler = assignHandler;
		_sessionRepo = sessionRepo;
	}

	public async Task HandleAsync(ProcessNextSessionCommand command)
	{
		var session = await _getNextSession.HandleAsync(new GetNextUnassignedSessionQuery());
		if (session is null)
			return;

		var agents = await _getActiveAgents.HandleAsync(new GetActiveAgentsQuery());

		var normalCapacity = agents.Sum(a => a.MaxConcurrentChats);
		var queueLength = await _sessionRepo.CountQueuedAsync();


		var pool = _overflow.ShouldOverflow(queueLength, normalCapacity)
				 ? await _overflow.GetOverflowAgentsAsync()
				 : agents;

		var chosen = await _strategy.SelectAgentAsync(session, pool);
		if (chosen is null)
			return;


		await _assignHandler.HandleAsync(new AssignSessionCommand(session.Id, chosen.Id));
	}
}
