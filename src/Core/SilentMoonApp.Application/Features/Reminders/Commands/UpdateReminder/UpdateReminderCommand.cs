using SilentMoonApp.Application.Abstractions.Messaging;


namespace SilentMoonApp.Application.Features.Reminders.Commands.UpdateReminder;

public sealed record UpdateReminderCommand(Guid Id,
										   string? Time,
										   IReadOnlyList<EWeekDay>? DaysOfWeek,
										   string? Label,
										   bool? IsEnabled):ICommand<UpdateReminderResult>;

