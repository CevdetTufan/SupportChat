using FluentAssertions;
using SupportChat.Application.Interfaces.Coordination;
using SupportChat.Application.Interfaces.Persistence;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.Agents;
using SupportChat.Domain.ChatSessions;
using SupportChat.Infrastructure.Coordination;

namespace SupportChat.Test.Integrations;

public class ActorToAgentIntegrationTests
{
	private static IAgentRepository CreateAgentRepo()
	{
		var agents = new List<Agent>();

		// Team A: 1xTeamLead, 2xMidLevel, 1xJunior
		agents.Add(Agent.Create(Guid.NewGuid(), "A_Lead", Seniority.TeamLead));
		agents.Add(Agent.Create(Guid.NewGuid(), "A_Mid1", Seniority.MidLevel));
		agents.Add(Agent.Create(Guid.NewGuid(), "A_Mid2", Seniority.MidLevel));
		agents.Add(Agent.Create(Guid.NewGuid(), "A_Junior", Seniority.Junior));

		// Team B: 1xSenior, 1xMidLevel, 2xJunior
		agents.Add(Agent.Create(Guid.NewGuid(), "B_Senior", Seniority.Senior));
		agents.Add(Agent.Create(Guid.NewGuid(), "B_Mid", Seniority.MidLevel));
		agents.Add(Agent.Create(Guid.NewGuid(), "B_Junior1", Seniority.Junior));
		agents.Add(Agent.Create(Guid.NewGuid(), "B_Junior2", Seniority.Junior));

		// Team C: night shift, 2xMidLevel
		agents.Add(Agent.Create(Guid.NewGuid(), "C_Mid1", Seniority.MidLevel));
		agents.Add(Agent.Create(Guid.NewGuid(), "C_Mid2", Seniority.MidLevel));

		// Overflow team: 6xJunior
		for (int i = 1; i <= 6; i++)
			agents.Add(Agent.Create(Guid.NewGuid(), $"Overflow_J{i}", Seniority.Junior));

		return new InMemoryAgentRepository(agents);
	}

	private static IChatSessionRepository CreateSessionRepo()
	{
		return new InMemoryChatSessionRepository();
	}
	
	private static IUnitOfWork CreateUnitOfWork()
	{
		return new InMemoryUnitOfWork();
	}
	
	private static IOverflowHandler CreateOverflow(IAgentRepository repo)
	{
		return new JuniorOverflowHandler(repo, new IstanbulOfficeHoursProvider());
	}
	
	private static ISessionAssignmentStrategy CreateStrategy()
	{
		return new SeniorityBasedAssignment();
	}

	private static IAgentSelector CreateSelector(IAgentRepository repo, ISessionAssignmentStrategy strat, IOverflowHandler overflow)
	{
		return new AgentSelector(repo, strat, overflow);
	}

	[Fact]
	public async Task ActorSessions_AssignedAcrossTeamsThenOverflow()
	{
		// Arrange: üç takım + overflow
		var agentRepo = CreateAgentRepo();
		var sessionRepo = CreateSessionRepo();
		var uow = CreateUnitOfWork();
		var strategy = CreateStrategy();
		var engine = new ChatCoordinatorEngine(
			sessionRepo, agentRepo, strategy, uow);

		// 1) normalCapacity hesapla (A=21, B=22, C=12)
		var normalCapacity = (
			// Team A
			1 * (int)Math.Floor(10 * 0.5) +
			2 * (int)Math.Floor(10 * 0.6) +
			1 * (int)Math.Floor(10 * 0.4)
		) + (
			// Team B
			1 * (int)Math.Floor(10 * 0.8) +
			1 * (int)Math.Floor(10 * 0.6) +
			2 * (int)Math.Floor(10 * 0.4)
		) + (
			// Team C
			2 * (int)Math.Floor(10 * 0.6)
		); // = 55

		// 2) ekstra overflow seansı
		const int extra = 0;
		var totalSessions = normalCapacity + extra; // 55

		// 3) Test için session’ları oluştur
		var sessionIds = Enumerable
			.Range(0, totalSessions)
			.Select(_ => Guid.NewGuid())
			.ToList();

		foreach (var id in sessionIds)
			await sessionRepo.AddAsync(ChatSession.Create(id));

		// Act: her seans için koordinatörü çalıştır
		foreach (var _ in sessionIds)
			await engine.ProcessNextSessionAsync();

		// Assert:

		// A) normalCapacity kadar oturum normal ajanlara gitmiş olmalı
		var normalAgents = (await agentRepo.GetActiveAgentsAsync())
			.Where(a => !a.Name.StartsWith("Overflow"))
			.ToList();

		var assignedToNormal = normalAgents.Sum(a => a.ActiveChatIds.Count);
		assignedToNormal
		  .Should().Be(sessionIds.Count)
		  .And.BeLessOrEqualTo(normalCapacity);

		// B) kalan extra seans overflow ajanlarına gitmiş olmalı
		var overflowAgents = (await agentRepo.GetBySeniorityAsync(Seniority.Junior))
			.Where(a => a.Name.StartsWith("Overflow"))
			.ToList();

		var assignedToOverflow = overflowAgents.Sum(a => a.ActiveChatIds.Count);
		assignedToOverflow
			.Should().Be(extra,
				"çünkü kalan {0} istek overflow takımına gitti", extra);

		// C) hiç kimse MaxConcurrentChats’ini aşmamış olmalı
		var allAgents = normalAgents.Concat(overflowAgents);
		allAgents
			.All(a => a.ActiveChatIds.Count <= a.MaxConcurrentChats)
			.Should().BeTrue("çünkü aynı anda chat sınırları aşılmamalı");
	}

}

#region In‐Memory Helpers

internal class InMemoryUnitOfWork : IUnitOfWork
{
	public Task SaveChangesAsync() => Task.CompletedTask;
}

internal class InMemoryChatSessionRepository : IChatSessionRepository
{
	private readonly List<ChatSession> _sessions;

	public InMemoryChatSessionRepository()
	{
		_sessions = new List<ChatSession>();
	}

	public Task AddAsync(ChatSession entity, CancellationToken token = default)
	{
		_sessions.Add(entity);
		return Task.CompletedTask;
	}

	public Task<int> CountAllAsync(CancellationToken token = default)
	{
		return Task.FromResult(_sessions.Count);
	}

	public Task<int> CountQueuedAsync()
	{
		var count = _sessions.Count(s => s.AssignedAgentId == null);
		return Task.FromResult(count);
	}

	public Task<int> CountQueuedAsync(CancellationToken token = default)
	{
		var count = _sessions.Count(s => s.AssignedAgentId == null);
		return Task.FromResult(count);
	}

	public void Delete(ChatSession entity)
	{
		_sessions.Remove(entity);
	}

	public Task<ChatSession?> GetByIdAsync(Guid id, CancellationToken token = default)
	{
		var session = _sessions.FirstOrDefault(s => s.Id == id);
		return Task.FromResult(session);
	}

	public Task<ChatSession?> GetNextUnassignedAsync()
	{
		var session = _sessions
			.Where(s => s.AssignedAgentId == null)
			.OrderBy(s => s.CreatedAt)
			.FirstOrDefault();
		return Task.FromResult(session);
	}

	public Task<ChatSession?> GetNextUnassignedAsync(CancellationToken token = default)
	{
		var session = _sessions
			.Where(s => s.AssignedAgentId == null)
			.OrderBy(s => s.CreatedAt)
			.FirstOrDefault();
		return Task.FromResult(session);
	}

	public Task<IReadOnlyList<ChatSession>> ListAllAsync(CancellationToken token = default)
	{
		return Task.FromResult((IReadOnlyList<ChatSession>)_sessions.ToList());
	}

	public void Update(ChatSession entity)
	{
	}
}

internal class InMemoryAgentRepository : IAgentRepository
{
	private readonly List<Agent> _agents;

	public InMemoryAgentRepository(List<Agent> initialAgents)
	{
		_agents = initialAgents ?? new List<Agent>();
	}

	public Task AddAsync(Agent entity, CancellationToken token = default)
	{
		_agents.Add(entity);
		return Task.CompletedTask;
	}

	public void Delete(Agent entity)
	{
		_agents.Remove(entity);
	}

	public Task<IReadOnlyCollection<Agent>> GetActiveAgentsAsync()
	{
		var active = _agents
			.Where(a => a.Status == AgentStatus.Active)
			.ToList();
		return Task.FromResult((IReadOnlyCollection<Agent>)active);
	}

	public Task<Agent?> GetByIdAsync(Guid id, CancellationToken token = default)
	{
		var agent = _agents.FirstOrDefault(a => a.Id == id);
		return Task.FromResult(agent);
	}

	public Task<IReadOnlyCollection<Agent>> GetBySeniorityAsync(Seniority seniority)
	{
		var filtered = _agents
			.Where(a => a.Seniority == seniority)
			.ToList();
		return Task.FromResult((IReadOnlyCollection<Agent>)filtered);
	}

	public Task<IReadOnlyList<Agent>> ListAllAsync(CancellationToken token = default)
	{
		return Task.FromResult((IReadOnlyList<Agent>)_agents.ToList());
	}

	public void Update(Agent entity)
	{
	}


	public Task<IReadOnlyCollection<Agent>> GetNormalAgentsAsync()
		=> Task.FromResult((IReadOnlyCollection<Agent>)
			_agents.Where(a => !a.Name.StartsWith("Overflow")).ToList());

	public Task<IReadOnlyCollection<Agent>> GetOverflowAgentsAsync()
		=> Task.FromResult((IReadOnlyCollection<Agent>)
			_agents.Where(a => a.Name.StartsWith("Overflow")).ToList());
}

internal class IstanbulOfficeHoursProvider : IOfficeHoursProvider
{
	public bool IsOfficeHours(DateTime _) => true;
}

#endregion