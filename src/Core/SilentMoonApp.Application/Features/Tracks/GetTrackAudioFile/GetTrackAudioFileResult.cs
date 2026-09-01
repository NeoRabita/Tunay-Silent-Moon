using SilentMoonApp.Application.DTOs.Storage;

namespace SilentMoonApp.Application.Features.Tracks.GetTrackAudioFile;

public sealed record GetTrackAudioFileResult
(
	string FileName,
	string ContentType,
	StorageStreamResult StorageStreamResult
);