namespace SupportChat.Domain.Agents;

public static class SeniorityExtensions
{
	public static double GetEfficiency(this Seniority seniority) => seniority switch
	{
		Seniority.Junior => 0.4,
		Seniority.MidLevel => 0.6,
		Seniority.Senior => 0.8,
		Seniority.TeamLead => 0.5,
		_ => throw new ArgumentOutOfRangeException(nameof(seniority))
	};
}
