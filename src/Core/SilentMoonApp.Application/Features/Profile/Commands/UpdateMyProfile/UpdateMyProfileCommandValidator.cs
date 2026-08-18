using FluentValidation;

namespace SilentMoonApp.Application.Features.Profile.Commands.UpdateMyProfile;

public sealed class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
	public UpdateMyProfileCommandValidator()
	{
		RuleFor(command => command.Name)

			.NotEmpty()
			.WithMessage("Ad bos ola bilm?z.")

			.MaximumLength(50)
			.WithMessage("Ad maksimum 50 simvol ola bil?r.");

	}
}
