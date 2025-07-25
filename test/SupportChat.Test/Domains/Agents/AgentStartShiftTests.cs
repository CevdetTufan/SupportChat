﻿using SupportChat.Domain.Agents;

namespace SupportChat.Test.Domains.Agents;

public class AgentStartShiftTests
{
	private Guid _teamId = Guid.NewGuid();
	[Fact]
	public void StartShift_WhenInactive_ShouldBecomeActive()
	{
		// Arrange
		var agent = Agent.Create(Guid.NewGuid(), "Test Agent", Seniority.Junior, _teamId);

		agent.EndShift();

		typeof(Agent)
			.GetProperty("Status")
			.SetValue(agent, AgentStatus.Inactive);

		// Act
		agent.StartShift();

		// Assert
		Assert.Equal(AgentStatus.Active, agent.Status);
	}

	[Theory]
	[InlineData(AgentStatus.Active)]
	[InlineData(AgentStatus.FinishingShift)]
	public void StartShift_WhenNotInactive_ShouldThrow(AgentStatus initialStatus)
	{
		// Arrange
		var agent = Agent.Create(Guid.NewGuid(), "Test Agent", Seniority.MidLevel, _teamId);

		typeof(Agent)
			.GetProperty("Status")
			.SetValue(agent, initialStatus);

		// Act & Assert
		var ex = Assert.Throws<InvalidOperationException>(() => agent.StartShift());
		Assert.Equal("Cannot start shift unless agent is inactive.", ex.Message);
	}


	[Fact]
	public void EndShift_WhenActive_ShouldBecomeFinishingShift()
	{
		// Arrange
		var agent = Agent.Create(Guid.NewGuid(), "Test Agent", Seniority.MidLevel, _teamId);

		Assert.Equal(AgentStatus.Active, agent.Status);

		// Act
		agent.EndShift();

		// Assert
		Assert.Equal(AgentStatus.FinishingShift, agent.Status);
	}

	[Theory]
	[InlineData(AgentStatus.FinishingShift)]
	[InlineData(AgentStatus.Inactive)]
	public void EndShift_WhenNotActive_ShouldThrow(AgentStatus initialStatus)
	{
		// Arrange
		var agent = Agent.Create(Guid.NewGuid(), "Test Agent", Seniority.MidLevel, _teamId);

		typeof(Agent)
			.GetProperty(nameof(Agent.Status))
			.SetValue(agent, initialStatus);

		// Act & Assert
		var ex = Assert.Throws<InvalidOperationException>(() => agent.EndShift());
		Assert.Equal("Can only end shift when agent is active.", ex.Message);
	}
}
