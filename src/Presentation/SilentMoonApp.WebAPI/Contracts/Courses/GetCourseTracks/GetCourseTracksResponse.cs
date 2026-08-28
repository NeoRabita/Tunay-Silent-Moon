namespace SilentMoonApp.WebAPI.Contracts.Courses.GetCourseTracks;

public sealed class GetCourseTracksResponse
{
	public IReadOnlyList<GetCourseTrackResponse> Tracks { get; init; } = [];
}


public sealed class GetCourseTrackResponse
{
	public Guid Id { get; init; }
	public Guid CourseId { get; init; }
	public string Title { get; init; } = null!;
	public Guid NarratorId { get; init; }
	public string NarratorName { get; init; } = null!;
	public string NarratorSlug { get; init; } = null!;
	public int DurationSec { get; init; }
	public string AudioUrl { get; init; } = string.Empty;
	public string MimeType { get; init; } = null!;
	public long FileSizeBytes { get; init; }
	public string ImageUrl { get; init; } = string.Empty;
	public int TrackNumber { get; init; }
}
