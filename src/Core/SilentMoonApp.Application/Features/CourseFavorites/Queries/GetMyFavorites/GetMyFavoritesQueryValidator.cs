using FluentValidation;

namespace SilentMoonApp.Application.Features.CourseFavorites.Queries.GetMyFavorites;

public sealed class GetMyFavoritesQueryValidator : AbstractValidator<GetMyFavoritesQuery>
{
	public GetMyFavoritesQueryValidator()
	{
		RuleFor(query => query.PaginationQueryRequest.PageNumber)
			.GreaterThan(0);

		RuleFor(query => query.PaginationQueryRequest.PageSize)
			.GreaterThan(0)
			.LessThanOrEqualTo(20);

		When(query => query.CategoryTypeId.HasValue, () =>
		{
			RuleFor(query => query.CategoryTypeId!.Value)
				.NotEmpty();
		});
	}
}