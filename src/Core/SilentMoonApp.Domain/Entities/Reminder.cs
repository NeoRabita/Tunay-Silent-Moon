using SilentMoonApp.Domain.Entities.Identity;

namespace SilentMoonApp.Domain;

public class Reminder : BaseEntity, IAuditableEntity
{
	// Properties
	public TimeSpan Time { get; set; }
	public int DaysOfWeek { get; set; }
	public string Label { get; set; } = string.Empty;
	public bool IsEnabled { get; set; }


	// Auditable Properties
	public Guid? CreatedBy { get; set; }
	public Guid? UpdatedBy { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset? UpdatedAt { get; set; }


	// Relations
	public Guid UserId { get; set; }
	public User User { get; set; } = null!;
}
