using SupportChat.Domain.Agents;

namespace SupportChat.Application.Interfaces.Repositories;

public interface IAgentRepository : IRepository<Agent>
{
	/// <summary>
	/// Sadece normal (Team A/B/C) takımdaki, yani overflow olmayan ajanları döner.
	/// </summary>
	Task<IReadOnlyCollection<Agent>> GetNormalAgentsAsync();

	/// <summary>
	/// Sadece overflow takımındaki ajanları döner.
	/// </summary>
	Task<IReadOnlyCollection<Agent>> GetOverflowAgentsAsync();

	/// <summary>
	/// Tüm aktif ajanları döner (sadece test veya genel durumlar için).
	/// </summary>
	Task<IReadOnlyCollection<Agent>> GetActiveAgentsAsync();

	Task<IReadOnlyCollection<Agent>> GetBySeniorityAsync(Seniority seniority);
}
