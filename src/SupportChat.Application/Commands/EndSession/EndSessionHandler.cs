using SupportChat.Application.Interfaces.Commands;
using SupportChat.Application.Interfaces.Persistence;
using SupportChat.Application.Interfaces.Repositories;

namespace SupportChat.Application.Commands.EndSession;

public class EndSessionHandler : ICommandHandler<EndSessionCommand>
{
	private readonly IChatSessionRepository _sessionRepo;
	private readonly IAgentRepository _agentRepo;
	private readonly IUnitOfWork _unitOfWork;

	public EndSessionHandler(
		IChatSessionRepository sessionRepo,
		IAgentRepository agentRepo,
		IUnitOfWork unitOfWork)
	{
		_sessionRepo = sessionRepo;
		_agentRepo = agentRepo;
		_unitOfWork = unitOfWork;
	}

	public async Task HandleAsync(EndSessionCommand command)
	{
		var session = await _sessionRepo.GetByIdAsync(command.SessionId)
					  ?? throw new InvalidOperationException($"Session {command.SessionId} not found.");

		session.End(); 

		// If the session was assigned to an agent, release it
		if (session.AssignedAgentId.HasValue)
		{
			var agent = await _agentRepo.GetByIdAsync(session.AssignedAgentId.Value)
					   ?? throw new InvalidOperationException($"Agent {session.AssignedAgentId} not found.");
			agent.ReleaseChat(session.Id);
			_agentRepo.Update(agent);
		}

		_sessionRepo.Update(session);
		await _unitOfWork.SaveChangesAsync();
	}
}
