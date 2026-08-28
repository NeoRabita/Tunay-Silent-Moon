
using SilentMoonApp.Domain.Entities.Files;

namespace SilentMoonApp.Domain.Entities;

public class Category : BaseEntity,
						IAuditableEntity,
						ISoftDeletableEntity
{
	// Properties
	public string Title { get; set; } = string.Empty;
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
	public Guid CategoryTypeId { get; set; }
	public CategoryType CategoryType { get; set; } = null!;

	public Guid? IconFileId { get; set; }
	public ImageFile? IconFile { get; set; }

	public ICollection<Course> Courses { get; set; } = new List<Course>();
}
