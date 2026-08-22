using SilentMoonApp.Application.Abstractions.Messaging;


namespace SilentMoonApp.Application.Features.Reminders.Commands.CreateReminder;

public sealed record CreateReminderCommand(string Time,
										   IReadOnlyList<EWeekDay> DaysOfWeek,
										   string Label) : ICommand<CreateReminderResult>;
