namespace SilentMoonApp.Application.Features.Reminders.Queries.GetMyReminders;

public sealed record GetMyRemindersResult
(
	Guid Id,
	string Time,
	IReadOnlyList<EWeekDay> DaysOfWeek,
	string Label,
	bool IsEnabled,
	DateTimeOffset CreatedAt
);
