using FluentValidation;
using SilentMoonApp.Application.Features.Auth.Commands.FacebookAuth;

public sealed class FacebookAuthCommandValidator : AbstractValidator<FacebookAuthCommand>
{
	public FacebookAuthCommandValidator()
	{
		RuleFor(command => command.IdToken)

			.NotEmpty()
			.WithMessage("Facebook ID token bos ola bilm?z.")

			.Must(idToken => !string.IsNullOrWhiteSpace(idToken))
			.WithMessage("Facebook ID token yalniz whitespace " +
						 "simvollarindan ibar?t ola bilm?z.")

			.MaximumLength(10000)
			.WithMessage("Facebook ID token maksimum 10000 " +
						 "simvol ola bil?r.")

			.Must(idToken => !idToken.Any(char.IsWhiteSpace))
			.WithMessage("Facebook ID token whitespace " +
						 "simvolu ehtiva ed? bilm?z.");
	}
}