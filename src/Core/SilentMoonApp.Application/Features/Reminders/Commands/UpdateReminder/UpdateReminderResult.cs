namespace SilentMoonApp.Application.Features.Reminders.Commands.UpdateReminder;

public sealed record UpdateReminderResult
(
	Guid Id,
	string Time,
	IReadOnlyList<EWeekDay> DaysOfWeek,
	string Label,
	bool IsEnabled,
	DateTimeOffset CreatedAt
);