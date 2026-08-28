using SilentMoonApp.Domain.Entities.Files;

namespace SilentMoonApp.Domain.Entities;

public class Track : BaseEntity,
					 IAuditableEntity,
					 ISoftDeletableEntity
{
	// Properties
	public string Title { get; set; } = string.Empty;
	public int Order { get; set; }


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
	public Guid CourseId { get; set; }
	public Course Course { get; set; } = null!;

	public Guid NarratorId { get; set; }
	public Narrator Narrator { get; set; } = null!;

	public Guid? CoverImageFileId { get; set; }
	public ImageFile? CoverImageFile { get; set; }

	public Guid AudioFileId { get; set; }
	public AudioFile AudioFile { get; set; } = null!;

	public ICollection<TrackProgress> TrackProgresses { get; set; } = new List<TrackProgress>();
}
