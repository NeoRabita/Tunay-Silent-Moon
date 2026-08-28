using SilentMoonApp.Domain.Entities.Identity;

namespace SilentMoonApp.Domain.Entities;

public class TrackProgress : BaseEntity
{
	// Properties
	public int PositionSec { get; set; }
	public bool Completed { get; set; }
	public DateTimeOffset UpdatedAt { get; set; }


	// Relations
	public Guid UserId { get; set; }
	public User User { get; set; } = null!;

	public Guid TrackId { get; set; }
	public Track Track { get; set; } = null!;
}