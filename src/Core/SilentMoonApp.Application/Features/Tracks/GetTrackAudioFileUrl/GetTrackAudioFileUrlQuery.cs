using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Tracks.GetTrackAudioFileUrl;

public sealed record GetTrackAudioFileUrlQuery(Guid TrackId):IQuery<GetTrackAudioFileUrlResult>;
