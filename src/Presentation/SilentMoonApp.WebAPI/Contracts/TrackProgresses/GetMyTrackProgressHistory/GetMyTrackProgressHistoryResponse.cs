using SilentMoonApp.WebAPI.Contracts.TrackProgresses.GetMyTrackProgress;

namespace SilentMoonApp.WebAPI.Contracts.TrackProgresses.GetMyTrackProgressHistory;


public sealed class GetMyTrackProgressHistoryItemResponse
{
	public GetMyTrackProgressResponse Progress { get; init; } = null!;
	public GetMyTrackProgressHistoryTrackResponse Track { get; init; } = null!;
}


public sealed class GetMyTrackProgressHistoryTrackResponse
{
	public Guid Id { get; init; }
	public Guid CourseId { get; init; }
	public string Title { get; init; } = string.Empty;
	public string Narrator { get; init; } = string.Empty;
	public int DurationSec { get; init; }
	public string AudioUrl { get; init; } = string.Empty;
	public string MimeType { get; init; } = string.Empty;
	public long FileSizeBytes { get; init; }
	public string ImageUrl { get; init; } = string.Empty;
	public int TrackNumber { get; init; }
}