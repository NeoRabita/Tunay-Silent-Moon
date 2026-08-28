namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourseTrackById;

public sealed record GetCourseTrackByIdResult
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
