using FluentValidation;
using SilentMoonApp.SharedKernel.Resources;


namespace SilentMoonApp.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
	public RegisterCommandValidator()
	{
		RuleFor(command => command.FirstName)

			.NotEmpty()
			.WithMessage("The Name field is required.")

			.MaximumLength(50)
			.WithMessage("The Name field can be at most 50 characters long.");


		RuleFor(command => command.LastName)

			.MaximumLength(50)
			.WithMessage("The Last Name field can be at most 50 characters long.");


		RuleFor(command => command.UserName)

			.MinimumLength(3)
			.WithMessage("The UserName field must be at least 3 characters long.")

			.MaximumLength(30)
			.WithMessage("The UserName field can be at most 30 characters long.")

			.Matches("^[a-zA-Z0-9._]+$")
			.WithMessage("The UserName field can only contain letters, numbers, dots, and underscores.");


		RuleFor(command => command.Email)

			.NotEmpty()
			.WithMessage(ErrorMessages.ValidationEmailRequired)

			.EmailAddress()
			.WithMessage(ErrorMessages.ValidationEmailInvalid)

			.MinimumLength(12)
			.WithMessage("E-mail Address must be at least 3 characters long.")

			.MaximumLength(254)
			.WithMessage("E-mail Address field can be at most 50 characters long.");


		RuleFor(command => command.Password)

			.NotEmpty()
			.WithMessage(ErrorMessages.ValidationPasswordRequired)

			.MinimumLength(8)
			.WithMessage("The Password  must be at least 3 characters long.")

			.MaximumLength(100)
			.WithMessage("Parol can be at most 50 characters long.")

			.Matches("[A-Z]")
			.WithMessage("The Password must contain at least one uppercase letter.")

			.Matches("[a-z]")
			.WithMessage("The Password must contain at least one lowercase letter.")

			.Matches("[0-9]")
			.WithMessage("The Password must contain at least one digit.");
	}

}
