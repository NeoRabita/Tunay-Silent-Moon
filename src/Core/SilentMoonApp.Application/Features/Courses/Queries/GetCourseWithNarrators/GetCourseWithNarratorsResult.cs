namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourseWithNarrators;

public sealed record GetCourseWithNarratorsResult
(
	Guid Id,
	string Title,
	string SubTitle,
	string CategoryType,
	Guid CategoryId,
	string ImageUrl,
	int DurationSec,
	bool IsFeatured,
	IReadOnlyList<string> Narrators,
	string Description,
	int TrackCount,
	DateTimeOffset CreatedAt
);