using SupportChat.Application.Interfaces.Commands;
using SupportChat.Application.Interfaces.Persistence;
using SupportChat.Application.Interfaces.Repositories;

namespace SupportChat.Application.Commands.Agents;

internal class AssignSessionHandler : ICommandHandler<AssignSessionCommand>
{
	private readonly IChatSessionRepository _sessionRepo;
	private readonly IAgentRepository _agentRepo;
	private readonly IUnitOfWork _uow;

	public AssignSessionHandler(IChatSessionRepository sessionRepo, IAgentRepository agentRepo, IUnitOfWork uow)
	{
		_sessionRepo = sessionRepo;
		_agentRepo = agentRepo;
		_uow = uow;
	}

	public async Task HandleAsync(AssignSessionCommand command)
	{
		var session = await _sessionRepo.GetByIdAsync(command.SessionId)
			?? throw new InvalidOperationException($"Session {command.SessionId} not found.");


		var agent = await _agentRepo.GetByIdAsync(command.AgentId)
					?? throw new InvalidOperationException($"Agent {command.AgentId} not found.");


		session.AssignToAgent(agent.Id);
		agent.AssignChat(session.Id);

		_sessionRepo.Update(session);
		_agentRepo.Update(agent);

		await _uow.SaveChangesAsync();
	}
}
