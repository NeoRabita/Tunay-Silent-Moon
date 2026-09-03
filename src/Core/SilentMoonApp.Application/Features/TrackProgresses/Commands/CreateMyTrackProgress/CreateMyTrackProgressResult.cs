namespace SilentMoonApp.Application.Features.TrackProgresses.Commands.CreateMyTrackProgress;

public sealed record CreateMyTrackProgressResult
(
	Guid Id,
	Guid TrackId,
	int PositionSec,
	bool Completed,
	DateTimeOffset UpdatedAt
);