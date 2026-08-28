using SilentMoonApp.Domain.Enums;


namespace SilentMoonApp.WebAPI.Contracts.Reminders.GetMyReminders;

public sealed class GetMyRemindersResponse
{
	public IReadOnlyList<GetMyReminderResponse> Reminders { get; init; } = Array.Empty<GetMyReminderResponse>();
}


public sealed class GetMyReminderResponse
{
	public required Guid Id { get; init; }
	public required string Time { get; init; }
	public required IReadOnlyList<EWeekDay> DaysOfWeek { get; init; }
	public required string Label { get; init; }
	public required bool IsEnabled { get; init; }
	public required DateTimeOffset CreatedAt { get; init; }
}

