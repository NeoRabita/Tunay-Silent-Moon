using FluentValidation;

namespace SilentMoonApp.Application.Features.TrackProgresses.Commands.CreateMyTrackProgress;

public sealed class CreateMyTrackProgressCommandValidator:AbstractValidator<CreateMyTrackProgressCommand>
{
	public CreateMyTrackProgressCommandValidator()
	{
		RuleFor(command => command.TrackId)
			.NotEmpty();

		RuleFor(command => command.PositionSec)
			.GreaterThanOrEqualTo(0);
	}
}
