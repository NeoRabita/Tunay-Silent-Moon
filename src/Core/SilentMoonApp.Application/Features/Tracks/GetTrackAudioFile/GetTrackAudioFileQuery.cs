using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Tracks.GetTrackAudioFile;

public sealed record GetTrackAudioFileQuery(Guid TrackId,
											string? RangeHeader) : IQuery<GetTrackAudioFileResult>;
