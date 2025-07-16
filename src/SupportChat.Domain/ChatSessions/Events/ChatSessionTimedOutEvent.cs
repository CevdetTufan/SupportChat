namespace SupportChat.Domain.ChatSessions.Events;

public record ChatSessionTimedOutEvent(
	Guid Id, 
	DateTime UtcNow);
