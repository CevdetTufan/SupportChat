namespace SupportChat.Domain.ChatSessions.Exceptions;

public class SessionPollCannotInactiveException : Exception
{
	public SessionPollCannotInactiveException()
	{
	}
	public SessionPollCannotInactiveException(string message)
		: base(message)
	{
	}
	public SessionPollCannotInactiveException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
