using SupportChat.Domain.Agents;

namespace SupportChat.Application.Interfaces.Repositories;

public interface IAgentRepository : IRepository<Agent>
{
	Task<IReadOnlyCollection<Agent>> GetActiveAgentsAsync();
	Task<IReadOnlyCollection<Agent>> GetBySeniorityAsync(Seniority seniority);
}
