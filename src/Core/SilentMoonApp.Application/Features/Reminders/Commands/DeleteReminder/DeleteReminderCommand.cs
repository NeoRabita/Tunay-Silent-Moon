using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Reminders.Commands.DeleteReminder;

public sealed record DeleteReminderCommand(Guid Id) : ICommand;
