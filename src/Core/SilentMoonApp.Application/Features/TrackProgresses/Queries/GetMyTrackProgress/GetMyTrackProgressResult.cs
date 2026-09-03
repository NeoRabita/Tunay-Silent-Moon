namespace SilentMoonApp.Application.Features.TrackProgresses.Queries.GetMyTrackProgress;

public sealed record GetMyTrackProgressResult
(
	Guid Id,
	Guid TrackId,
	int PositionSec,
	bool Completed,
	DateTimeOffset UpdatedAt
);