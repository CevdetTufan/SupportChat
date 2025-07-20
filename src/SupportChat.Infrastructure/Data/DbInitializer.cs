using SupportChat.Domain.Agents;

namespace SupportChat.Infrastructure.Data;

public static class DbInitializer
{
	public static void Initialize(AppDbContext ctx)
	{
		ctx.Database.EnsureCreated();

		if (ctx.Teams.Any())
			return;

		//teams
		var teamA = new Team(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Team A", 8, 16);
		var teamB = new Team(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Team B", 8, 16);
		var teamC = new Team(Guid.Parse("33333333-3333-3333-3333-333333333333"), "Team C", 16, 0);
		var overflow = new Team(Guid.Parse("AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA"), "Overflow", 8, 16);

		ctx.Teams.AddRange(teamA, teamB, teamC, overflow);

		// Agents
		ctx.Agents.AddRange(
			// Team A: 1 Lead, 2 Mid, 1 Junior
			Agent.Create(Guid.NewGuid(), "A_Lead", Seniority.TeamLead, teamA.Id),
			Agent.Create(Guid.NewGuid(), "A_Mid_1", Seniority.MidLevel, teamA.Id),
			Agent.Create(Guid.NewGuid(), "A_Mid_2", Seniority.MidLevel, teamA.Id),
			Agent.Create(Guid.NewGuid(), "A_Junior_1", Seniority.Junior, teamA.Id),

			// Team B: 1 Senior, 1 Mid, 2 Junior
			Agent.Create(Guid.NewGuid(), "B_Senior", Seniority.Senior, teamB.Id),
			Agent.Create(Guid.NewGuid(), "B_Mid", Seniority.MidLevel, teamB.Id),
			Agent.Create(Guid.NewGuid(), "B_Junior_1", Seniority.Junior, teamB.Id),
			Agent.Create(Guid.NewGuid(), "B_Junior_2", Seniority.Junior, teamB.Id),

			// Team C (night shift): 2 Mid
			Agent.Create(Guid.NewGuid(), "C_Mid_1", Seniority.MidLevel, teamC.Id),
			Agent.Create(Guid.NewGuid(), "C_Mid_2", Seniority.MidLevel, teamC.Id),

			// Overflow: 6 Junior
			Agent.Create(Guid.NewGuid(), "OF1", Seniority.Junior, overflow.Id),
			Agent.Create(Guid.NewGuid(), "OF2", Seniority.Junior, overflow.Id),
			Agent.Create(Guid.NewGuid(), "OF3", Seniority.Junior, overflow.Id),
			Agent.Create(Guid.NewGuid(), "OF4", Seniority.Junior, overflow.Id),
			Agent.Create(Guid.NewGuid(), "OF5", Seniority.Junior, overflow.Id),
			Agent.Create(Guid.NewGuid(), "OF6", Seniority.Junior, overflow.Id)
		);

		ctx.SaveChanges();
	}
}