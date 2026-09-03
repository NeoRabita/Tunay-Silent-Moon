using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.TrackProgresses.Queries.GetMyTrackProgress;

public sealed record GetMyTrackProgressQuery(Guid TrackId):IQuery<GetMyTrackProgressResult>;
