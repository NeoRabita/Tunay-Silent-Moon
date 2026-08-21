using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.WebAPI.Contracts.Reminders.CreateReminder;

public sealed class CreateReminderResponse
{
	public required Guid Id { get; init; }
	public required string Time { get; init; }
	public required IReadOnlyList<EWeekDay> DaysOfWeek { get; init; }
	public required string Label { get; init; }
	public required bool IsEnabled { get; init; }
	public required DateTimeOffset CreatedAt { get; init; }
}
