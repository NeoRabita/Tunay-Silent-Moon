using FluentValidation;

namespace SilentMoonApp.Application.Features.CourseFavorites.Commands.DeleteMyFavorite;

public sealed class DeleteMyFavoriteCommandValidator : AbstractValidator<DeleteMyFavoriteCommand>
{
	public DeleteMyFavoriteCommandValidator()
	{
		RuleFor(command => command.CourseId)
			.NotEmpty();
	}
}
