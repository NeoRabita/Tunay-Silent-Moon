using SilentMoonApp.Domain.Entities.Files;

namespace SilentMoonApp.Domain.Entities;

public class Course : BaseEntity,
					  IAuditableEntity,
					  ISoftDeletableEntity
{
	// Properties
	public string Title { get; set; } = string.Empty;
	public string SubTitle { get; set; } = string.Empty;
	public string Description { get; set; } = string.Empty;

	public int DurationSec { get; set; }
	public bool IsFeatured { get; set; }


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
	public Guid CategoryId { get; set; }
	public Category Category { get; set; } = null!;

	public Guid CoverImageFileId { get; set; }
	public ImageFile CoverImageFile { get; set; } = null!;

	public ICollection<Track> Tracks { get; set; } = new List<Track>();
	public ICollection<CourseFavorite> CourseFavorites { get; set; } = new List<CourseFavorite>();
}
