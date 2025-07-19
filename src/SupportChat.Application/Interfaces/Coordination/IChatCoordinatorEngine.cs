namespace SupportChat.Application.Interfaces.Coordination;

public interface IChatCoordinatorEngine
{
	Task ProcessNextSessionAsync();
}
