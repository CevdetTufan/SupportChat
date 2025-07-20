using Microsoft.EntityFrameworkCore;
using SupportChat.Domain.Agents;
using SupportChat.Domain.ChatSessions;

namespace SupportChat.Infrastructure.Data;

public class AppDbContext : DbContext
{
	public DbSet<Team> Teams => Set<Team>();
	public DbSet<Agent> Agents => Set<Agent>();
	public DbSet<ChatSession> ChatSessions => Set<ChatSession>();

	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Team>(b =>
		{
			b.HasKey(t => t.Id);
			b.Property(t => t.Name).HasMaxLength(20).IsRequired();
			b.Property(t => t.ShiftStartHour);
			b.Property(t => t.ShiftEndHour);
		});

		modelBuilder.Entity<Agent>(b =>
		{
			b.HasKey(a => a.Id);
			b.Property(a => a.Name).HasMaxLength(100).IsRequired();
			b.Property(a => a.Seniority).IsRequired();
			b.Property(a => a.Status).IsRequired();
			b.Property(a => a.TeamId).IsRequired();

			b.HasOne(a => a.Team)
			 .WithMany()
			 .HasForeignKey(a => a.TeamId);
		});

		modelBuilder.Entity<ChatSession>(b =>
		{
			b.HasKey(cs => cs.Id);
			b.Property(cs => cs.Status).IsRequired();
			b.Property(cs => cs.AssignedAgentId).IsRequired(false);
			b.Property(cs => cs.CreatedAt).IsRequired();
			b.Property(cs => cs.LastPolledAt).IsRequired();
			b.HasOne<Agent>()
			 .WithMany()
			 .HasForeignKey(cs => cs.AssignedAgentId)
			 .OnDelete(DeleteBehavior.SetNull);
		});
	}
}
