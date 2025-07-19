using Microsoft.EntityFrameworkCore;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.Agents;
using SupportChat.Infrastructure.Data;

namespace SupportChat.Infrastructure.Repositories;

internal class AgentRepository : Repository<Agent>, IAgentRepository
{
	private readonly AppDbContext _db;
	public AgentRepository(AppDbContext context) : base(context)
	{
		_db = context;
	}

	public async Task<IReadOnlyCollection<Agent>> GetActiveAgentsAsync()
	{
		return await _db.Agents
			.Where(a => a.Status == AgentStatus.Active)
			.ToListAsync();
	}

	public async Task<IReadOnlyCollection<Agent>> GetBySeniorityAsync(Seniority seniority)
	{
		return await _db.Agents
			.Where(a => a.Seniority == seniority)
			.ToListAsync();
	}
}
