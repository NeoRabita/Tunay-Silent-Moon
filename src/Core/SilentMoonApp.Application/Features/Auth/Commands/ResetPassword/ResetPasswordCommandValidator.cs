using FluentValidation;
using SilentMoonApp.SharedKernel.Resources;


namespace SilentMoonApp.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
	public ResetPasswordCommandValidator()
	{
		RuleFor(user => user.Email)

			.NotEmpty()
			.WithMessage(ErrorMessages.ValidationEmailRequired)

			.EmailAddress()
			.WithMessage(ErrorMessages.ValidationEmailInvalid)

			.MinimumLength(12)
			.WithMessage("E-poçt ünvani minimum 12 simvol olmalidir.")

			.MaximumLength(254)
			.WithMessage("E-poçt ünvani maksimum 254 simvol ola bil?r.");


		RuleFor(x => x.OtpCode)
			.NotEmpty()
			.WithMessage("OTP kodu t?l?b olunur.")

			.Matches(@"^\d{6}$")
			.WithMessage("OTP kodu 6 r?q?md?n ibar?t olmalidir.");


		RuleFor(command => command.NewPassword)

			.NotEmpty()
			.WithMessage("Parol bos ola bilm?z.")

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


		RuleFor(command => command.ConfirmPassword)

			.NotEmpty()
			.WithMessage("Parol bos ola bilm?z.")

			.Equal(command => command.NewPassword)
			.WithMessage("Yeni parol v? t?krar parol eyni deyil.");
	}
}
