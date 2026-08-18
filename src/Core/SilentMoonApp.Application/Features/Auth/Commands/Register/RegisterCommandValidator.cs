using FluentValidation;
using SilentMoonApp.SharedKernel.Resources;


namespace SilentMoonApp.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
	public RegisterCommandValidator()
	{
		RuleFor(command => command.FirstName)
			
			.NotEmpty()
			.WithMessage("Ad bos ola bilm?z.")
			
			.MaximumLength(50)
			.WithMessage("Ad maksimum 50 simvol ola bil?r.");


		RuleFor(command => command.LastName)
			
			.MaximumLength(50)
			.WithMessage("Soyad maksimum 50 simvol ola bil?r.");


		RuleFor(command => command.UserName)

			.MinimumLength(3)
			.WithMessage("Istifad?çi adi minimum 3 simvol olmalidir.")

			.MaximumLength(30)
			.WithMessage("Istifad?çi adi maksimum 30 simvol ola bil?r.")

			.Matches("^[a-zA-Z0-9._]+$")
			.WithMessage("Istifad?çi adinda yalniz h?rf, r?q?m, nöqt? v? alt x?tt istifad? edil? bil?r.");


		RuleFor(command => command.Email)

			.NotEmpty()
			.WithMessage(ErrorMessages.ValidationEmailRequired)

			.EmailAddress()
			.WithMessage(ErrorMessages.ValidationEmailInvalid)

			.MinimumLength(12)
			.WithMessage("E-poçt ünvani minimum 12 simvol olmalidir.")

			.MaximumLength(254)
			.WithMessage("E-poçt ünvani maksimum 254 simvol ola bil?r.");


		RuleFor(command => command.Password)

			.NotEmpty()
			.WithMessage(ErrorMessages.ValidationPasswordRequired)

			.MinimumLength(8)
			.WithMessage("Parol minimum 8 simvol olmalidir.")

			.MaximumLength(100)
			.WithMessage("Parol maksimum 100 simvol ola bil?r.")

			.Matches("[A-Z]")
			.WithMessage("Parolda ?n azi bir böyük h?rf olmalidir.")

			.Matches("[a-z]")
			.WithMessage("Parolda ?n azi bir kiçik h?rf olmalidir.")

			.Matches("[0-9]")
			.WithMessage("Parolda ?n azi bir r?q?m olmalidir.");
	}
}
