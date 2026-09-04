
using FluentValidation;

namespace SilentMoonApp.Application.Features.SearchCourses.Queries.GetSearchCourses;

public sealed class GetSearchCoursesQueryValidator : AbstractValidator<GetSearchCoursesQuery>
{
	public GetSearchCoursesQueryValidator()
	{
		RuleFor(query => query.Search)
			.NotEmpty();

		RuleFor(query => query.PaginationQueryRequest.PageNumber)
			.GreaterThan(0);

		RuleFor(query => query.PaginationQueryRequest.PageSize)
			.GreaterThan(0)
			.LessThanOrEqualTo(50);
	}
}