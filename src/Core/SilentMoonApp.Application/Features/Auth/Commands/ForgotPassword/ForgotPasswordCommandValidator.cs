using FluentValidation;
using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
	public ForgotPasswordCommandValidator()
	{
		RuleFor(command => command.Email)

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
