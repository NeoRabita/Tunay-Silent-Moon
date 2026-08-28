namespace SilentMoonApp.Domain.Entities;

public class Narrator : BaseEntity,
						IAuditableEntity,
						ISoftDeletableEntity
{
	// Properties
	public string Name { get; set; } = string.Empty;
	public string Slug { get; set; } = string.Empty;


	// Auditable Properties
	public Guid? CreatedBy { get; set; }
	public Guid? UpdatedBy { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset? UpdatedAt { get; set; }


	// Soft Delete Properties
	public bool IsDeleted { get; set; } = false;
	public Guid? DeletedBy { get; set; }
	public DateTimeOffset? DeletedAt { get; set; }


	// Relations
	public ICollection<Track> Tracks { get; set; } = new List<Track>();
}
