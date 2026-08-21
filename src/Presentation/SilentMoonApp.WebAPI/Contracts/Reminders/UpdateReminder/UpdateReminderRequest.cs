using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.WebAPI.Contracts.Reminders.UpdateReminder;

public sealed class UpdateReminderRequest
{
	public  string? Time { get; init; }
	public  IReadOnlyList<EWeekDay>? DaysOfWeek { get; init; }
	public  string? Label { get; init; }
	public  bool? IsEnabled { get; init; }
}
