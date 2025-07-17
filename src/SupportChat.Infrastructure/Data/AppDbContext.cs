using Microsoft.EntityFrameworkCore;
using SupportChat.Domain.Agents;
using SupportChat.Domain.ChatSessions;

namespace SupportChat.Infrastructure.Data;

public class AppDbContext : DbContext
{
	public DbSet<Agent> Agents => Set<Agent>();
	public DbSet<ChatSession> ChatSessions => Set<ChatSession>();

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		base.OnModelCreating(modelBuilder);
	}
}
