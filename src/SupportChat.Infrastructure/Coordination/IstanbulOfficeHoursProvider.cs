using SupportChat.Application.Interfaces.Coordination;

namespace SupportChat.Infrastructure.Coordination;

internal class IstanbulOfficeHoursProvider : IOfficeHoursProvider
{
	private static readonly TimeZoneInfo _ist = TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
	private static readonly TimeSpan _start = TimeSpan.FromHours(9);
	private static readonly TimeSpan _end = TimeSpan.FromHours(18);

	public bool IsOfficeHours(DateTime utcNow)
	{
		var local = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _ist).TimeOfDay;
		return local >= _start && local < _end;
	}
}
