using SilentMoonApp.Application.DTOs.Common;


namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourses;

public sealed record GetCoursesResult
(
	PaginationResult<GetCourseItemResult> PaginationResult
);


public sealed record GetCourseItemResult
(
	Guid Id,
	string Title,
	string SubTitle,
	string CategoryType,
	string CategoryName,
	Guid? CategoryId,
	string? CoverImageFileUrl,
	int DurationSec,
	bool IsFeatured,
	IReadOnlyList<string> Narrators
);