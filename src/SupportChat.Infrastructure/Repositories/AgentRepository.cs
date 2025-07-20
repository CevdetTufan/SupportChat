using Microsoft.EntityFrameworkCore;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.Agents;
using SupportChat.Infrastructure.Data;
using System.Runtime.InteropServices;

namespace SupportChat.Infrastructure.Repositories;

internal class AgentRepository : Repository<Agent>, IAgentRepository
{
	private readonly AppDbContext _db;

	private static readonly TimeZoneInfo _ist =
			RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
				? TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time")
				: TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");

	public AgentRepository(AppDbContext context) : base(context)
	{
		_db = context;
	}

	public async Task<IReadOnlyCollection<Agent>> GetActiveAgentsAsync()
	{
		var localHour = TimeZoneInfo
		.ConvertTimeFromUtc(DateTime.UtcNow, _ist)  
		.Hour;

		return await _db.Agents
			.Include(a => a.Team)
			.Where(a => a.Status == AgentStatus.Active)
			.Where(a =>
				a.Team.ShiftStartHour < a.Team.ShiftEndHour
					? localHour >= a.Team.ShiftStartHour && localHour < a.Team.ShiftEndHour
					: localHour >= a.Team.ShiftStartHour || localHour < a.Team.ShiftEndHour
			)
			.ToListAsync();
	}

	public async Task<IReadOnlyCollection<Agent>> GetBySeniorityAsync(Seniority seniority)
	{
		return await _db.Agents
			.Where(a => a.Seniority == seniority)
			.ToListAsync();
	}

	public async Task<IReadOnlyCollection<Agent>> GetNormalAgentsAsync()
	{
		var localHour = TimeZoneInfo
			.ConvertTimeFromUtc(DateTime.UtcNow, _ist)  
			.Hour;

		return await _db.Agents
			.Include(a => a.Team)
			.Where(a => a.Status == AgentStatus.Active)
			.Where(a => !a.Name.StartsWith("Overflow"))
			.Where(a =>
				a.Team.ShiftStartHour < a.Team.ShiftEndHour
					? localHour >= a.Team.ShiftStartHour && localHour < a.Team.ShiftEndHour
					: localHour >= a.Team.ShiftStartHour || localHour < a.Team.ShiftEndHour
			).ToListAsync();
	}

	public async Task<IReadOnlyCollection<Agent>> GetOverflowAgentsAsync()
	{
		var localHour = TimeZoneInfo
			.ConvertTimeFromUtc(DateTime.UtcNow, _ist)  
			.Hour;

		return await _context.Agents
			  .Include(a => a.Team)
			  .Where(a => a.Name.StartsWith("Overflow"))
			  .Where(a =>
				a.Team.ShiftStartHour < a.Team.ShiftEndHour
					? localHour >= a.Team.ShiftStartHour && localHour < a.Team.ShiftEndHour
					: localHour >= a.Team.ShiftStartHour || localHour < a.Team.ShiftEndHour
			).ToListAsync();
	}
}
