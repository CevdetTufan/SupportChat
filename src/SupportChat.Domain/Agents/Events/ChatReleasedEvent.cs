namespace SupportChat.Domain.Agents.Events;

public record ChatReleasedEvent(
	Guid AgentId, 
	Guid SessionId, 
	DateTime ReleasedAt
	);
