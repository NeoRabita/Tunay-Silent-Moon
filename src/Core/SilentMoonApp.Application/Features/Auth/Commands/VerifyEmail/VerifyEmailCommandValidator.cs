using FluentValidation;
using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Features.Auth.Commands.VerifyEmail;

public sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
	public VerifyEmailCommandValidator()
	{
		RuleFor(user => user.Email)
			.NotEmpty()
			.WithMessage(ErrorMessages.ValidationEmailRequired)

			.EmailAddress()
			.WithMessage(ErrorMessages.ValidationEmailInvalid)

			.MinimumLength(12)
			.WithMessage("E-poçt ünvani minimum 12 simvol olmalidir.");


		RuleFor(x => x.OtpCode)
			.NotEmpty()
			.WithMessage("OTP kodu t?l?b olunur.")

			.Matches(@"^\d{6}$")
			.WithMessage("OTP kodu 6 r?q?md?n ibar?t olmalidir.");
	}
}
