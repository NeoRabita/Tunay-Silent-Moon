using FluentValidation;

namespace SilentMoonApp.Application.Features.CourseFavorites.Commands.CreateMyFavorite;

public sealed class CreateMyFavoriteCommandValidator : AbstractValidator<CreateMyFavoriteCommand>
{
	public CreateMyFavoriteCommandValidator()
	{
		RuleFor(command => command.CourseId)
			.NotEmpty();
	}
}
