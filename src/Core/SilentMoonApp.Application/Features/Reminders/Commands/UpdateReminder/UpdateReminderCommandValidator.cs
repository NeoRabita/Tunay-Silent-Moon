using FluentValidation;

namespace SilentMoonApp.Application.Features.Reminders.Commands.UpdateReminder;

public class UpdateReminderCommandValidator:AbstractValidator<UpdateReminderCommand>
{
	public UpdateReminderCommandValidator()
	{
		//RuleFor(command => command.Id)
			
		//	.NotEmpty()
		//	.WithMessage("Reminder id tələb olunur.");


		//RuleFor(command => command)

		//	.Must(command =>
		//		  command.Time is not null ||
		//		  command.DaysOfWeek is not null ||
		//		  command.Label is not null ||
		//		  command.IsEnabled.HasValue)

		//	.WithMessage("Yenilənəcək ən azı bir məlumat göndərilməlidir.");


		//When(command => command.Time is not null, () =>
		//{
		//	RuleFor(command => command.Time!)

		//		.NotEmpty()
		//		.WithMessage("Reminder vaxtı boş ola bilməz.")

		//		.Matches(@"^([01]\d|2[0-3]):[0-5]\d$")
		//		.WithMessage("Vaxt HH:mm formatında olmalıdır.");
		//});


		//When(command => command.DaysOfWeek is not null, () =>
		//{
		//	RuleFor(command => command.DaysOfWeek!)

		//		.NotEmpty()
		//		.WithMessage("Ən azı bir gün seçilməlidir.")

		//		.Must(days => days.Distinct().Count() == days.Count)
		//		.WithMessage("Eyni gün bir neçə dəfə seçilə bilməz.");


		//	RuleForEach(command => command.DaysOfWeek!)

		//		.IsInEnum()
		//		.WithMessage("Gün yalnız 1 ilə 7 arasında ola bilər.");
		//});


		//When(command => command.Label is not null, () =>
		//{
		//	RuleFor(command => command.Label!)

		//		.NotEmpty()
		//		.WithMessage("Label boş ola bilməz.")

		//		.MaximumLength(100)
		//		.WithMessage("Label maksimum 100 simvol ola bilər.");
		//});
	}
}
