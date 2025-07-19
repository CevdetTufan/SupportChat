using Microsoft.EntityFrameworkCore;
using SupportChat.Application.Interfaces.Repositories;
using SupportChat.Domain.ChatSessions;
using SupportChat.Infrastructure.Data;

namespace SupportChat.Infrastructure.Repositories;

internal class ChatSessionRepository : Repository<ChatSession>, IChatSessionRepository
{
	private readonly AppDbContext _db;
	public ChatSessionRepository(AppDbContext context) : base(context)
	{
		_db = context;
	}

	public async Task<int> CountAllAsync(CancellationToken token = default)
	{
		return await _db.ChatSessions.CountAsync(token);
	}

	public async Task<int> CountQueuedAsync(CancellationToken token = default)
	{
		return await _db.ChatSessions
			   .CountAsync(s => s.AssignedAgentId == null, token);
	}

	public async Task<ChatSession?> GetNextUnassignedAsync(CancellationToken token = default)
	{
		return await _db.ChatSessions
			   .Where(s => s.AssignedAgentId == null)
			   .OrderBy(s => s.CreatedAt)
			   .FirstOrDefaultAsync(token);
	}
}
