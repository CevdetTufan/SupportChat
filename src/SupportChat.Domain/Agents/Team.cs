namespace SupportChat.Domain.Agents;

public class Team
{
	public Guid Id { get; private set; }
	public string Name { get; private set; }

	public int ShiftStartHour { get; private set; }
	public int ShiftEndHour { get; private set; }

	private Team() { }

	public Team(Guid id, string name, int shiftStartHour, int shiftEndHour)
	{
		if (shiftStartHour < 0 || shiftStartHour > 23)
			throw new ArgumentOutOfRangeException(nameof(shiftStartHour), "Shift start hour must be between 0 and 23.");

		Id = id;
		Name = name;
		ShiftStartHour = shiftStartHour;
		ShiftEndHour = shiftEndHour;
	}
}
