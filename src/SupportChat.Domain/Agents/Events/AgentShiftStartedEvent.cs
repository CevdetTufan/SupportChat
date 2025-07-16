namespace SupportChat.Domain.Agents.Events;

public record AgentShiftStartedEvent(
	Guid AgentId,
	DateTime StartedAt
	);
