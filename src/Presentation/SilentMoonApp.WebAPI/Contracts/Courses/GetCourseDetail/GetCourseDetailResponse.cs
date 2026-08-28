namespace SilentMoonApp.WebAPI.Contracts.Courses.GetCourseDetail;

public sealed class GetCourseDetailResponse
{
	public GetCourseDetailCourseResponse Course { get; init; } = null!;

	public IReadOnlyList<GetCourseDetailTrackResponse> Tracks { get; init; } = [];

	public GetCourseDetailUserProgressResponse? UserProgress { get; init; }

	public bool IsFavorited { get; init; }
}


public sealed class GetCourseDetailCourseResponse
{
	public Guid Id { get; init; }
	public string Title { get; init; } = null!;
	public string SubTitle { get; init; } = null!;
	public string CategoryType { get; init; } = null!;
	public Guid CategoryId { get; init; }
	public string ImageUrl { get; init; } = string.Empty;
	public int DurationSec { get; init; }
	public bool IsFeatured { get; init; }
	public IReadOnlyList<GetCourseDetailNarratorResponse> Narrators { get; init; } = [];
	public string Description { get; init; } = null!;
	public int TrackCount { get; init; }
	public DateTimeOffset CreatedAt { get; init; }
}


public sealed class GetCourseDetailNarratorResponse
{
	public Guid Id { get; init; }
	public string Name { get; init; } = null!;
	public string Slug { get; init; } = null!;
}


public sealed class GetCourseDetailTrackResponse
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


public sealed class GetCourseDetailUserProgressResponse
{
	public Guid Id { get; init; }
	public Guid TrackId { get; init; }
	public int PositionSec { get; init; }
	public bool Completed { get; init; }
	public DateTimeOffset UpdatedAt { get; init; }
}
