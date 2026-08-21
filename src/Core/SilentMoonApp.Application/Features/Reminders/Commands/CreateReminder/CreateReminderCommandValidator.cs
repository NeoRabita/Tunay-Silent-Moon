using FluentValidation;

namespace SilentMoonApp.Application.Features.Reminders.Commands.CreateReminder;

public sealed class CreateReminderCommandValidator:AbstractValidator<CreateReminderCommand>
{
	public CreateReminderCommandValidator()
	{
		RuleFor(command => command.Time)
			
			.NotEmpty()
			.WithMessage("Reminder Time is required.")

			.Matches(@"^([01]\d|2[0-3]):[0-5]\d$")
			.WithMessage("Time must be in HH:mm format.");


		RuleFor(command => command.DaysOfWeek)

			.NotEmpty()
			.WithMessage("At least one day must be selected.")

			.Must(days => days.Distinct().Count() == days.Count)
			.WithMessage("The same day cannot be selected more than once.");


		RuleForEach(command => command.DaysOfWeek)

			.IsInEnum()
			.WithMessage("Day must be between 1 and 7.");
	}
}

