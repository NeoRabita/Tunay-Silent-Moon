using FluentValidation;

namespace SilentMoonApp.Application.Features.Auth.Commands.GoogleAuth;

public sealed class GoogleAuthCommandValidator : AbstractValidator<GoogleAuthCommand>
{
	public GoogleAuthCommandValidator()
	{
		RuleFor(command => command.IdToken)

			.NotEmpty()
			.WithMessage("Google ID token bos ola bilm?z.")

			.Must(idToken => !string.IsNullOrWhiteSpace(idToken))
			.WithMessage("Google ID token yalniz whitespace simvollarindan ibar?t ola bilm?z.")

			.MaximumLength(10000)
			.WithMessage($"Google ID token maksimum 10000 simvol ola bil?r.")

			.Must(idToken => !idToken.Any(char.IsWhiteSpace))
			.WithMessage("Google ID token whitespace simvolu ehtiva ed? bilm?z.");
	}
}
