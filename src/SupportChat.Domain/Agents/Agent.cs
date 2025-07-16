namespace SupportChat.Domain.Agents;

public class Agent
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public Seniority Seniority { get; set; }
	public double Efficiency { get; set; }
	public AgentStatus Status { get; set; }
}
