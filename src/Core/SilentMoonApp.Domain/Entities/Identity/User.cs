using SilentMoonApp.Domain.Entities.Common;
using SilentMoonApp.Domain.Entities.Files;
using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.Domain.Entities.Identity;

public class User : BaseEntity,
					IAuditableEntity,
					ISoftDeletableEntity
{
	// Properties
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string? UserName { get; set; }

	public string Email { get; set; } = string.Empty;
	public string PasswordHash { get; set; } = string.Empty;
	public bool IsEmailConfirmed { get; set; } = false;

	public int AccessFailedCount { get; set; } = 0;
	public DateTimeOffset? LockoutEndAt { get; set; }
	public DateTimeOffset? ConfirmedAt { get; set; }

	public EUserStatus UserStatus { get; set; } = EUserStatus.PendingVerification;


	// Auditable Properties
	public Guid? CreatedBy { get; set; }
	public Guid? UpdatedBy { get; set; }
	public DateTimeOffset CreatedAt { get; set; } 
	public DateTimeOffset? UpdatedAt { get; set; }


	// Soft-Deletable Properties
	public bool IsDeleted { get; set; } = false;
	public Guid? DeletedBy { get; set; }
	public DateTimeOffset? DeletedAt { get; set; }


	// Relations
	public Guid? AvatarImageFileId { get; set; }
	public ImageFile? AvatarImageFile { get; set; }

	public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
	public ICollection<UserExternalProvider> UserExternalProviders { get; set; } = new List<UserExternalProvider>();
	public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
