namespace SupportChat.Application.Interfaces.Coordination;

public interface IOfficeHoursProvider
{
	bool IsOfficeHours(DateTime utcNow);
}
