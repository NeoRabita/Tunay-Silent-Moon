using SilentMoonApp.Application.DTOs.Common;


namespace SilentMoonApp.Application.Features.SearchCourses.Queries.GetSearchCourses;

public sealed record GetSearchCoursesResult
(
	string Search,
	PaginationResult<GetSearchCourseItemResult> PaginationResult
);


public sealed record GetSearchCourseItemResult
(
	Guid Id,
	string Title,
	string SubTitle,
	string Type,
	Guid? CategoryId,
	string ImageUrl,
	int DurationSec,
	bool IsFeatured,
	IReadOnlyList<string> Narrators
);
