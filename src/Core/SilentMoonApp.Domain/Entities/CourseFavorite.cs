using SilentMoonApp.Domain.Entities.Identity;

namespace SilentMoonApp.Domain.Entities;

public class CourseFavorite : BaseEntity
{
	// Properties
	public DateTimeOffset CreatedAt { get; set; }


	// Relations
	public Guid UserId { get; set; }
	public User User { get; set; } = null!;

	public Guid CourseId { get; set; }
	public Course Course { get; set; } = null!;
}
