using SupportChat.Domain.ChatSessions;
using SupportChat.Domain.ChatSessions.Exceptions;

namespace SupportChat.Test.Domains.ChatSessions;

public class ChatSessionTests
{
	[Fact]
	public void Create_WithValidId_ShouldInitializeQueuedSession()
	{
		// Arrange
		var id = Guid.NewGuid();

		// Act
		var session = ChatSession.Create(id);

		// Assert
		Assert.NotNull(session);
		Assert.Equal(id, session.Id);
		Assert.Equal(ChatStatus.Queued, session.Status);
		Assert.Null(session.AssignedAgentId);
		Assert.True((DateTime.UtcNow - session.CreatedAt).TotalSeconds < 1);
		Assert.Equal(session.CreatedAt, session.LastPolledAt);
	}

	[Fact]
	public void Poll_WhenQueuedOrAssigned_ShouldUpdateLastPolledAt()
	{
		// Arrange
		var session = ChatSession.Create(Guid.NewGuid());
		var before = session.LastPolledAt;

		// Act
		session.Poll();

		// Assert
		Assert.True(session.LastPolledAt > before);
	}

	[Fact]
	public void Poll_WhenInactive_ShouldThrow()
	{
		// Arrange
		var session = ChatSession.Create(Guid.NewGuid());
		session.MarkInactive();

		// Act & Assert
		Assert.Throws<SessionPollCannotInactiveException>(() => session.Poll());
	}

	[Fact]
	public void AssignToAgent_FromQueued_ShouldSetAssigned()
	{
		// Arrange
		var session = ChatSession.Create(Guid.NewGuid());
		var agentId = Guid.NewGuid();

		// Act
		session.AssignToAgent(agentId);

		// Assert
		Assert.Equal(ChatStatus.Assigned, session.Status);
		Assert.Equal(agentId, session.AssignedAgentId);
	}

	[Theory]
	[InlineData(ChatStatus.Assigned)]
	[InlineData(ChatStatus.Inactive)]
	public void AssignToAgent_WhenNotQueued_ShouldThrow(ChatStatus startStatus)
	{
		// Arrange
		var session = ChatSession.Create(Guid.NewGuid());

		typeof(ChatSession)
			.GetProperty(nameof(ChatSession.Status))
			.SetValue(session, startStatus);

		// Act & Assert
		Assert.Throws<SessionAlreadyAssignedException>(() => session.AssignToAgent(Guid.NewGuid()));
	}

	[Fact]
	public void MarkInactive_FromAnyState_ShouldSetInactive()
	{
		// Arrange
		var session = ChatSession.Create(Guid.NewGuid());
		session.AssignToAgent(Guid.NewGuid());

		// Act
		session.MarkInactive();

		// Assert
		Assert.Equal(ChatStatus.Inactive, session.Status);
	}

	[Fact]
	public void MarkInactive_Idempotent_WhenAlreadyInactive()
	{
		// Arrange
		var session = ChatSession.Create(Guid.NewGuid());
		session.MarkInactive();
		var firstTime = session.Status;

		// Act
		session.MarkInactive();

		// Assert
		Assert.Equal(firstTime, session.Status);
	}
}