using SilentMoonApp.Domain.Entities.Common;
using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.Domain.Entities.Identity;

public class UserExternalProvider : BaseEntity
{
	// Properties
	public EExternalAuthProvider Provider { get; set; }
	public string ProviderUserId { get; set; } = string.Empty;
	public DateTimeOffset CreatedAt { get; set; }


	// Relations
	public Guid UserId { get; set; }
	public User User { get; set; } = null!;
}
