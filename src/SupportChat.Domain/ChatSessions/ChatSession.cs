using SupportChat.Domain.ChatSessions.Events;

namespace SupportChat.Domain.ChatSessions;

public class ChatSession
{
	public Guid Id { get; private set; }
	public ChatStatus Status { get; private set; }
	public Guid? AssignedAgentId { get; private set; }
	public DateTime CreatedAt { get; private set; }
	public DateTime LastPolledAt { get; private set; }


	// Domain events
	private readonly List<object> _domainEvents = new();
	public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();


	private ChatSession(Guid id)
	{
		Id = id;
		Status = ChatStatus.Queued;
		CreatedAt = DateTime.UtcNow;
		LastPolledAt = CreatedAt;
		_domainEvents.Add(new ChatSessionCreatedEvent(id, CreatedAt));
	}

	public static ChatSession Create(Guid id)
	{
		return new ChatSession(id);
	}

	public void Poll()
	{
		if (Status == ChatStatus.Inactive)
			throw new InvalidOperationException("Cannot poll an inactive session.");
		LastPolledAt = DateTime.UtcNow;
	}

	public void AssignToAgent(Guid agentId)
	{
		if (Status != ChatStatus.Queued)
			throw new InvalidOperationException("Session already assigned or inactive.");
		AssignedAgentId = agentId;
		Status = ChatStatus.Assigned;
	}

	public void MarkInactive()
	{
		if (Status == ChatStatus.Inactive)
			return;
		Status = ChatStatus.Inactive;
		_domainEvents.Add(new ChatSessionTimedOutEvent(Id, DateTime.UtcNow));
	}

	public void End()
	{
		if (Status == ChatStatus.Inactive)
			throw new InvalidOperationException("Session is already ended.");

		Status = ChatStatus.Inactive;
	}

	public void ClearDomainEvents()
	{
		_domainEvents.Clear();
	}
}