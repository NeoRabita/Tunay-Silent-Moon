using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.WebAPI.Contracts.Reminders.CreateReminder;

public sealed class CreateReminderRequest
{
	public required string Time { get; init; }
	public required IReadOnlyList<EWeekDay> DaysOfWeek { get; init; }
	public required string Label { get; init; }
}
