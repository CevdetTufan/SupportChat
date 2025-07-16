namespace SupportChat.Domain.ChatSessions.Events;

public record ChatSessionCreatedEvent(
	Guid Id, 
	DateTime CreatedAt
	);
