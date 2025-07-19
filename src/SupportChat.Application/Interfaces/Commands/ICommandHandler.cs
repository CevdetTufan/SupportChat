namespace SupportChat.Application.Interfaces.Commands;

public interface ICommandHandler<in TCommand>
{
	Task HandleAsync(TCommand command);
}
