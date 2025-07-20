using SupportChat.Application.Commands.Exceptions;
using SupportChat.Application.Interfaces.Commands;
using SupportChat.Application.Interfaces.Persistence;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.ChatSessions;

namespace SupportChat.Application.Commands.CreateChatSession;

public class CreateChatSessionHandler : ICommandHandler<CreateChatSessionCommand>
{
	private readonly IChatSessionRepository _sessionRepo;
	private readonly IAgentRepository _agentRepo;
	private readonly IUnitOfWork _uow;

	public CreateChatSessionHandler(IChatSessionRepository sessionRepo, IAgentRepository agentRepo, IUnitOfWork uow)
	{
		_sessionRepo = sessionRepo;
		_agentRepo = agentRepo;
		_uow = uow;
	}

	public async Task HandleAsync(CreateChatSessionCommand cmd)
	{
		var normalAgents = await _agentRepo.GetNormalAgentsAsync();
		var capacity = normalAgents.Sum(a => a.MaxConcurrentChats);

		var queued = await _sessionRepo.CountQueuedAsync();
		var maxQueue = (int)Math.Floor(capacity * 1.5);

		if (queued >= maxQueue)
			throw new QueueFullException(
				$"Queue is full ({queued}/{maxQueue}).");

		var session = ChatSession.Create(cmd.SessionId);
		await _sessionRepo.AddAsync(session);
		await _uow.SaveChangesAsync();
	}
}
