namespace SupportChat.Application.Commands.AssignSession;

public record AssignSessionCommand(Guid SessionId, Guid AgentId);
