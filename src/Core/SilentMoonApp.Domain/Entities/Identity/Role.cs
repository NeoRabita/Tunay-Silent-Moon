using SilentMoonApp.Domain.Entities.Common;
using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.Domain.Entities.Identity;

public class Role : BaseEntity
{
	// Properties
	public EUserRole Name { get; set; }
	public string NormalizedName { get; set; } = string.Empty;
	public string? Description { get; set; }
	
	public DateTimeOffset CreatedAt { get; set; }


	// Relations
	public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
