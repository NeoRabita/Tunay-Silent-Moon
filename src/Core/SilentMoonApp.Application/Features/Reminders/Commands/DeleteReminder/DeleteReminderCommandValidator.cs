using FluentValidation;

namespace SilentMoonApp.Application.Features.Reminders.Commands.DeleteReminder;

public sealed class DeleteReminderCommandValidator : AbstractValidator<DeleteReminderCommand>
{
	public DeleteReminderCommandValidator()
	{
		RuleFor(command => command.Id)
			.NotEmpty()
			.WithMessage("Reminder is required.");
	}
}
