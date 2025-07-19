using SupportChat.Application.Interfaces.Commands;
using SupportChat.Application.Interfaces.Persistence;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.ChatSessions;

namespace SupportChat.Application.Commands.CreateChatSession;

public class CreateChatSessionHandler : ICommandHandler<CreateChatSessionCommand>
{
	private readonly IChatSessionRepository _repo;
	private readonly IUnitOfWork _uow;

	public CreateChatSessionHandler(IChatSessionRepository repo, IUnitOfWork uow)
	{
		_repo = repo;
		_uow = uow;
	}

	public async Task HandleAsync(CreateChatSessionCommand cmd)
	{
		var session = ChatSession.Create(cmd.SessionId);
		await _repo.AddAsync(session);
		await _uow.SaveChangesAsync();
	}
}
