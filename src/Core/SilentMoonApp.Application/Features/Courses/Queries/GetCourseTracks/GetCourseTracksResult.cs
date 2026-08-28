namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourseTracks;

public sealed record GetCourseTracksResult
(
	IReadOnlyList<GetCourseTrackItemResult> Tracks
);


public sealed record GetCourseTrackItemResult
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