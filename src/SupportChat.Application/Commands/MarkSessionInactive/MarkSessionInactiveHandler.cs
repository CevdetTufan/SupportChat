using SupportChat.Application.Interfaces.Commands;
using SupportChat.Application.Interfaces.Persistence;
using SupportChat.Application.Interfaces.Repositories;

namespace SupportChat.Application.Commands.MarkSessionInactive;

public class MarkSessionInactiveHandler : ICommandHandler<MarkSessionInactiveCommand>
{
	private readonly IChatSessionRepository _sessionRepo;
	private readonly IUnitOfWork _unitOfWork;

	public MarkSessionInactiveHandler(
		IChatSessionRepository sessionRepo,
		IUnitOfWork unitOfWork)
	{
		_sessionRepo = sessionRepo;
		_unitOfWork = unitOfWork;
	}

	public async Task HandleAsync(MarkSessionInactiveCommand command)
	{
		var session = await _sessionRepo.GetByIdAsync(command.SessionId)
					  ?? throw new InvalidOperationException($"Session {command.SessionId} not found.");

		session.MarkInactive(); 

		_sessionRepo.Update(session);
		await _unitOfWork.SaveChangesAsync();
	}
}
