namespace SupportChat.Domain.ChatSessions.Exceptions;
public class SessionAlreadyEndedException: Exception
{
	public SessionAlreadyEndedException()
	{
	}
	public SessionAlreadyEndedException(string message)
		: base(message)
	{
	}
	public SessionAlreadyEndedException(string message, Exception inner)
		: base(message, inner)
	{
	}
}
