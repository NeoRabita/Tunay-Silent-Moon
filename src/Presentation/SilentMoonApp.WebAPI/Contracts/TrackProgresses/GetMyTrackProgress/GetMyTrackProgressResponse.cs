namespace SilentMoonApp.WebAPI.Contracts.TrackProgresses.GetMyTrackProgress;

public sealed class GetMyTrackProgressResponse
{
	public Guid Id { get; init; }
	public Guid TrackId { get; init; }
	public int PositionSec { get; init; }
	public bool Completed { get; init; }
	public DateTimeOffset UpdatedAt { get; init; }
}
