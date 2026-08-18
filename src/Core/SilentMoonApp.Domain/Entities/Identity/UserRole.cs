using SilentMoonApp.Domain.Entities.Common;

namespace SilentMoonApp.Domain.Entities.Identity;

public class UserRole : BaseEntity
{
	// Relations
	public Guid UserId { get; set; }
	public User User { get; set; } = null!;

	public Guid RoleId { get; set; }
	public Role Role { get; set; } = null!;

	public DateTimeOffset CreatedAt { get; set; }
}
