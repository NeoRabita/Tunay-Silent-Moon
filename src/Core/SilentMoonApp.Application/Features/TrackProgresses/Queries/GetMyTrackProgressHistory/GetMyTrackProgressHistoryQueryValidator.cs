using FluentValidation;

namespace SilentMoonApp.Application.Features.TrackProgresses.Queries.GetMyTrackProgressHistory;

public class GetMyTrackProgressHistoryQueryValidator : AbstractValidator<GetMyTrackProgressHistoryQuery>
{
	public GetMyTrackProgressHistoryQueryValidator()
	{
		RuleFor(query => query.PaginationQueryRequest.PageNumber)
			.GreaterThan(0);

		RuleFor(query => query.PaginationQueryRequest.PageSize)
			.GreaterThan(0)
			.LessThanOrEqualTo(20);
	}
}
