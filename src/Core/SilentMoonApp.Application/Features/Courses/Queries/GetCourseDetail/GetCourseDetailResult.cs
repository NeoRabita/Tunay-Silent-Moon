namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourseDetail;

public sealed record GetCourseDetailResult
(
	GetCourseDetailCourseResult Course,
	IReadOnlyList<GetCourseDetailTrackResult> Tracks,
	GetCourseDetailUserProgressResult? UserProgress,
	bool IsFavorited
);


public sealed record GetCourseDetailCourseResult
(
	Guid Id,
	string Title,
	string SubTitle,
	string CategoryType,
	Guid CategoryId,
	string ImageUrl,
	int DurationSec,
	bool IsFeatured,
	IReadOnlyList<GetCourseDetailNarratorResult> Narrators,
	string Description,
	int TrackCount,
	DateTimeOffset CreatedAt
);


public sealed record GetCourseDetailNarratorResult
(
	Guid Id,
	string Name,
	string Slug
);


public sealed record GetCourseDetailTrackResult
(
	Guid Id,
	Guid CourseId,
	string Title,
	Guid NarratorId,
	string NarratorName,
	string NarratorSlug,
	int DurationSec,
	string AudioUrl,
	string MimeType,
	long FileSizeBytes,
	string ImageUrl,
	int TrackNumber
);


public sealed record GetCourseDetailUserProgressResult
(
	Guid Id,
	Guid TrackId,
	int PositionSec,
	bool Completed,
	DateTimeOffset UpdatedAt
);