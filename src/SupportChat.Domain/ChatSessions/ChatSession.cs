using SupportChat.Domain.ChatSessions.Exceptions;

namespace SupportChat.Domain.ChatSessions;

public class ChatSession
{
	public Guid Id { get; private set; }
	public ChatStatus Status { get; private set; }
	public Guid? AssignedAgentId { get; private set; }
	public DateTime CreatedAt { get; private set; }
	public DateTime LastPolledAt { get; private set; }


	private ChatSession(Guid id)
	{
		Id = id;
		Status = ChatStatus.Queued;
		CreatedAt = DateTime.UtcNow;
		LastPolledAt = CreatedAt;
	}

	public static ChatSession Create(Guid id)
	{
		return new ChatSession(id);
	}

	public void Poll()
	{
		if (Status == ChatStatus.Inactive)
			throw new SessionPollCannotInactiveException("Cannot poll an inactive session.");
		LastPolledAt = DateTime.UtcNow;
	}

	public void AssignToAgent(Guid agentId)
	{
		if (Status != ChatStatus.Queued)
			throw new SessionAlreadyAssignedException("Session already assigned or inactive.");
		AssignedAgentId = agentId;
		Status = ChatStatus.Assigned;
	}

	public void MarkInactive()
	{
		if (Status == ChatStatus.Inactive)
			return;
		Status = ChatStatus.Inactive;
	}

	public void End()
	{
		if (Status == ChatStatus.Inactive)
			throw new SessionAlreadyEndedException("Session is already ended.");

		Status = ChatStatus.Inactive;
	}
}