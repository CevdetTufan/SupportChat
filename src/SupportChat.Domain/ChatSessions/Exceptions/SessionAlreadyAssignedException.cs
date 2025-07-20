namespace SupportChat.Domain.ChatSessions.Exceptions;

public class SessionAlreadyAssignedException : Exception
{
	public SessionAlreadyAssignedException()
	{
	}

	public SessionAlreadyAssignedException(string message)
		: base(message)
	{
	}

	public SessionAlreadyAssignedException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
