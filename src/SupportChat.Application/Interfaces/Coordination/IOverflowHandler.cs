using SupportChat.Domain.Agents;

namespace SupportChat.Application.Interfaces.Coordination;

public interface IOverflowHandler
{
	bool ShouldOverflow(int currentQueueLength, int normalCapacity);

	Task<IReadOnlyCollection<Agent>> GetOverflowAgentsAsync();
}
