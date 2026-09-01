namespace SilentMoonApp.Application.Features.Tracks.GetTrackAudioFileUrl;

public sealed record GetTrackAudioFileUrlResult
(
	Guid TrackId,
	Guid CourseId,
	string TrackTitle,
	string FileName,
	string StreamUrl,
	DateTimeOffset ExpiresAt,
	string ContentType,
	long FileSizeBytes,
	int DurationSec
);