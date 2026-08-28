namespace SilentMoonApp.Application.Features.Courses.Queries.GetRelatedCourses;

public sealed record GetRelatedCoursesResult
(
	Guid Id,
	string Title,
	string SubTitle,
	string Type,
	Guid CategoryId,
	string ImageUrl,
	int DurationSec,
	bool IsFeatured,
	IReadOnlyList<string> Narrators
);