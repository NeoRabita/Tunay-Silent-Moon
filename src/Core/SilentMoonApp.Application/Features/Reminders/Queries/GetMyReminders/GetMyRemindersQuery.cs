using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Reminders.Queries.GetMyReminders;

public sealed record GetMyRemindersQuery() : IQuery<IReadOnlyList<GetMyRemindersResult>>;
