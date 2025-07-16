namespace SupportChat.Domain.Agents.Events;

public record ChatAssignedEvent(
	Guid AgentId,
	Guid SessionId, 
	DateTime AssignedAt
	);
