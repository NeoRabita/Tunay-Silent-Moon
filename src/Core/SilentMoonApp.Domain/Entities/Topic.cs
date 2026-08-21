
namespace SilentMoonApp.Domain.Entities;

public class Topic : BaseEntity,
					 IAuditableEntity,
					 ISoftDeletableEntity
{
	// Properties
	public string Slug { get; set; } = string.Empty;
	public string Title { get; set; } = string.Empty;
	public string IconUrl { get; set; } = string.Empty;
	public string ColorHex { get; set; } = string.Empty;


	// Auditable properties
	public Guid? CreatedBy { get; set; }
	public Guid? UpdatedBy { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset? UpdatedAt { get; set; }


	// Soft delete properties
	public bool IsDeleted { get; set; } = false;
	public Guid? DeletedBy { get; set; }
	public DateTimeOffset? DeletedAt { get; set; }


	// Relations
	public ICollection<UserTopic> UserTopics { get; set; } = new List<UserTopic>();
}
