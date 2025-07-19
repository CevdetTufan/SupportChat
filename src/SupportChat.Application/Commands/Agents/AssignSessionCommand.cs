namespace SupportChat.Application.Commands.Agents;

public record AssignSessionCommand(Guid SessionId, Guid AgentId);
