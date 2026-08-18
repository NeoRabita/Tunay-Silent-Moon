using FluentValidation;

namespace SilentMoonApp.Application.Features.Auth.Commands.Refresh;

public sealed class RefreshCommandValidator : AbstractValidator<RefreshCommand>
{
	public RefreshCommandValidator()
	{
		RuleFor(command => command.RefreshToken)

			.NotEmpty()
			.WithMessage("Refresh token bos ola bilm?z.")

			.MaximumLength(1024)
			.WithMessage($"Refresh token maksimum 1024 simvol ola bil?r.")

			.Must(token => string.IsNullOrEmpty(token) ||
						  !token.Any(char.IsWhiteSpace))
			.WithMessage("Refresh token bosluq simvollari ehtiva ed? bilm?z.");
	}
}

