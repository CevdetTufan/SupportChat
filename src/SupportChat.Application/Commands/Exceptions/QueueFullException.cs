namespace SupportChat.Application.Commands.Exceptions;

public class QueueFullException : Exception
{
	public QueueFullException(string message) : base(message) { }
}
