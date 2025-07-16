namespace SupportChat.Domain.Agents.Events;

public record AgentShiftCompletedEvent(
	Guid AgentId, 
	DateTime CompletedAt
	);
