using SupportChat.Application.Commands.CreateChatSession;
using SupportChat.Application.Interfaces.Commands;

namespace SupportChat.Api.Endpoints;

public static class ChatEndpoints
{
	public static void MapChatEndpoints(this IEndpointRouteBuilder app)
	{
		var chatGroup = app.MapGroup("/chat")
				   .WithTags("Chat");

		chatGroup.MapPost("/createSession", async (
				ICommandHandler<CreateChatSessionCommand> createHandler) =>
		{
			var sessionId = Guid.NewGuid();
			await createHandler.HandleAsync(new CreateChatSessionCommand(sessionId, DateTime.UtcNow));

			var location = $"/chat/{sessionId}";
			return Results.Created(location, new { SessionId = sessionId });
		})
		.WithName("StartChatSession");
	}
}
