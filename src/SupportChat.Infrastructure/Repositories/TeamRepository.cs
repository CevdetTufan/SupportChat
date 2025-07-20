using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.Agents;
using SupportChat.Infrastructure.Data;

namespace SupportChat.Infrastructure.Repositories;

internal class TeamRepository : Repository<Team>, ITeamRepository
{
	public TeamRepository(AppDbContext context) : base(context)
	{
	}
}
