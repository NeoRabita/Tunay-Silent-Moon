namespace SilentMoonApp.WebAPI.Contracts.Tracks.GetTrackAudioFile;

public sealed class GetTrackAudioFileUrlResponse
{
	public Guid TrackId { get; init; }
	public Guid CourseId { get; init; }
	public string TrackTitle { get; init; } = string.Empty;
	public string FileName { get; init; } = string.Empty;
	public string StreamUrl { get; init; } = string.Empty;
	public DateTimeOffset ExpiresAt { get; init; }
	public string ContentType { get; init; } = string.Empty;
	public long FileSizeBytes { get; init; }
	public int DurationSec { get; init; }
}
