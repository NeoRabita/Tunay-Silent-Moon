using FluentValidation;
using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Features.Auth.Commands.ResendEmailOtp;

public sealed class ResendEmailOtpCommandValidator : AbstractValidator<ResendEmailOtpCommand>
{
	public ResendEmailOtpCommandValidator()
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
	}

}
