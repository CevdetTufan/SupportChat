using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.Agents;
using SupportChat.Infrastructure.Data;

namespace SupportChat.Infrastructure.Repositories;

internal class AgentRepository : Repository<Agent>, IAgentRepository
{
	public AgentRepository(AppDbContext context) : base(context)
	{
	}
}
