namespace SilentMoonApp.WebAPI.Contracts.TrackProgresses.CreateMyTrackProgress;

public sealed class CreateMyTrackProgressRequest
{
	public Guid TrackId { get; init; }
	public int PositionSec { get; init; }
	public bool Completed { get; init; }
}
