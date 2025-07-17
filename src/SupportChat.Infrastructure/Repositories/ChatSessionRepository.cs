using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.ChatSessions;
using SupportChat.Infrastructure.Data;

namespace SupportChat.Infrastructure.Repositories;

internal class ChatSessionRepository : Repository<ChatSession>, IChatSessionRepository
{
	public ChatSessionRepository(AppDbContext context) : base(context)
	{
	}
}
