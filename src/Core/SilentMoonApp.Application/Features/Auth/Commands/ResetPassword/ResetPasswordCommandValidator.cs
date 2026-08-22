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
			.WithMessage("E-mail can be at least 12 characters long.")

			.MaximumLength(254)
			.WithMessage("E-mail can be at most 254 characters long.");



		RuleFor(x => x.OtpCode)
			.NotEmpty()
			.WithMessage("OTP is required.")

			.Matches(@"^\d{6}$")
			.WithMessage("OTP must be a 6-digit number.");


		
		RuleFor(command => command.NewPassword)

			.NotEmpty()
			.WithMessage("Password cannot be empty.")

			.MinimumLength(8)
			.WithMessage("Password must be at least 8 characters long.")

			.MaximumLength(100)
			.WithMessage("Password can be at most 100 characters long.")

			.Matches("[A-Z]")
			.WithMessage("Password must contain at least one uppercase letter.")

			.Matches("[a-z]")
			.WithMessage("Password must contain at least one lowercase letter.")

			.Matches("[0-9]")
			.WithMessage("Password must contain at least one number.");



		RuleFor(command => command.ConfirmPassword)

			.NotEmpty()
			.WithMessage("Password cannot be empty.")

			.Equal(command => command.NewPassword)
			.WithMessage("New password and confirm password do not match.");
	}
}
