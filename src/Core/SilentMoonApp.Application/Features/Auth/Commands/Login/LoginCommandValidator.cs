using FluentValidation;
using SilentMoonApp.SharedKernel.Resources;
 

namespace SilentMoonApp.Application.Features.Auth.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
	public LoginCommandValidator()
	{
		RuleFor(x => x.Email)

		    .NotEmpty()
		    .WithMessage(ErrorMessages.ValidationEmailRequired)
		    
		    .EmailAddress()
		    .WithMessage(ErrorMessages.ValidationEmailInvalid)
		    
		    .MinimumLength(12)
		    .WithMessage("E-poçt ünvani minimum 12 simvol olmalidir.")
		    
		    .MaximumLength(254)
		    .WithMessage("E-poçt ünvani maksimum 254 simvol ola bil?r.");


		RuleFor(x => x.Password)

			.NotEmpty()
			.WithMessage(ErrorMessages.ValidationPasswordRequired)

			.MinimumLength(8)
			.WithMessage("Parol minimum 8 simvol olmalidir.")

			.MaximumLength(100)
			.WithMessage("Parol maksimum 100 simvol ola bil?r.");
	}

}
