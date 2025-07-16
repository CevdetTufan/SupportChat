using SupportChat.Domain.Agents;

namespace SupportChat.Test.Domains;

public class AgentEntityTest
{
	[Theory]
	[InlineData(Seniority.Junior, 0.4, 4)]
	[InlineData(Seniority.MidLevel, 0.6, 6)]
	[InlineData(Seniority.Senior, 0.8, 8)]
	[InlineData(Seniority.TeamLead, 0.5, 5)]
	public void CreateAgent_ValidParameters_ShouldCreateAgent(Seniority seniority, double expectedEfficiency, int expectedMaxConcurrent)
	{
		// Arrange
		var id = Guid.NewGuid();
		var name = "Test Agent";

		// Act
		var agent =  Agent.Create(id, name, seniority);

		// Assert
		Assert.NotNull(agent);
		Assert.Equal(id, agent.Id);
		Assert.Equal(name, agent.Name);
		Assert.Equal(seniority, agent.Seniority);
		Assert.Equal(expectedEfficiency, agent.Efficiency, 3);
		Assert.Equal(expectedMaxConcurrent, agent.MaxConcurrentChats);
		Assert.Equal(AgentStatus.Active, agent.Status);
	}
}
