using SupportChat.Domain.Agents.Events;

namespace SupportChat.Domain.Agents;

public class Agent
{
	public Guid Id { get; private set; }
	public string Name { get; private set; }
	public Seniority Seniority { get; private set; }
	public double Efficiency { get; private set; }
	public AgentStatus Status { get; private set; }

	// Active chats
	private readonly List<Guid> _activeChatIds = new();
	public IReadOnlyCollection<Guid> ActiveChatIds => _activeChatIds;

	// Domain events
	private readonly List<object> _domainEvents = new();
	public IReadOnlyCollection<object> DomainEvents => _domainEvents.AsReadOnly();


	private Agent(Guid id, string name, Seniority seniority, AgentStatus status)
	{
		if (id == Guid.Empty)
			throw new ArgumentException("Id cannot be an empty Guid.", nameof(id));

		if (string.IsNullOrWhiteSpace(name))
			throw new ArgumentException("Name cannot be null or empty.", nameof(name));

		if (!Enum.IsDefined(typeof(Seniority), seniority))
			throw new ArgumentOutOfRangeException(nameof(seniority), "Invalid seniority level.");

		if (!Enum.IsDefined(typeof(AgentStatus), status))
			throw new ArgumentOutOfRangeException(nameof(status), "Invalid agent status.");

		Id = id;
		Name = name;
		Seniority = seniority;
		Efficiency = seniority.GetEfficiency();
		Status = status;
	}

	//Consider the maximum concurrency (how many chats each agent can handle at the same time) is 10.
	//This is multiplied by the efficiency, which is pegged to the seniority of the agent as per below
	public int MaxConcurrentChats
	{
		get
		{
			return (int)Math.Floor(10 * Efficiency);
		}
	}

	public bool CanAcceptChat()
	{
		return Status == AgentStatus.Active && _activeChatIds.Count < MaxConcurrentChats;
	}

	public static Agent Create(Guid id, string name, Seniority seniority)
	{
		var agent = new Agent(id, name, seniority, AgentStatus.Active);
		agent._domainEvents.Add(new AgentShiftStartedEvent(agent.Id, DateTime.UtcNow));
		return agent;
	}

	//Agents are on a 3 shift basis 8 hrs each.
	//When a shift is over, the agent must finish his current chats, but will not be assigned new chats.”
	public void StartShift()
	{
		if (Status != AgentStatus.Inactive)
			throw new InvalidOperationException("Cannot start shift unless agent is inactive.");
		Status = AgentStatus.Active;

		_domainEvents.Add(new AgentShiftStartedEvent(Id, DateTime.UtcNow));
	}

	//When a shift is over, the agent must finish his current chats, but will not be assigned new chats.
	public void EndShift()
	{
		if (Status != AgentStatus.Active)
			throw new InvalidOperationException("Can only end shift when agent is active.");
		Status = AgentStatus.FinishingShift;
	}

	//When a shift is over, the agent must finish his current chats, but will not be assigned new chats
	public void CompleteShift()
	{
		if (Status != AgentStatus.FinishingShift)
			throw new InvalidOperationException("Can only complete shift when finishing shift.");
		Status = AgentStatus.Inactive;

		_domainEvents.Add(new AgentShiftCompletedEvent(Id, DateTime.UtcNow));
	}

	public void AssignChat(Guid sessionId)
	{
		if (!CanAcceptChat())
			throw new InvalidOperationException("Agent cannot accept more chats.");
		_activeChatIds.Add(sessionId);
		_domainEvents.Add(new ChatAssignedEvent(Id, sessionId, DateTime.UtcNow));
	}

	public void ReleaseChat(Guid sessionId)
	{
		_activeChatIds.Remove(sessionId);
		if (Status == AgentStatus.FinishingShift && _activeChatIds.Count == 0)
			Status = AgentStatus.Inactive;

		_domainEvents.Add(new ChatAssignedEvent(Id, sessionId, DateTime.UtcNow));
	}

	public void ClearDomainEvents()
	{
		_domainEvents.Clear();
	}
}
