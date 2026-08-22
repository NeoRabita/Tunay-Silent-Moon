namespace SilentMoonApp.Application.Features.Reminders.Commands.CreateReminder;

public sealed record CreateReminderResult
(
	Guid Id,
	string Time,
	IReadOnlyList<EWeekDay> DaysOfWeek,
	string Label,
	bool IsEnabled,
	DateTimeOffset CreatedAt
);