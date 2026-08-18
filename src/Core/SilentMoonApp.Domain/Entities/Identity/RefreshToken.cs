using SilentMoonApp.Domain.Entities.Common;
using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.Domain.Entities.Identity;

public class RefreshToken : BaseEntity
{
	// Properties
	public string TokenHash { get; set; } = default!;
	public string? UserAgent { get; set; }
	public string? IpAddress { get; set; }

	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset ExpiresAt { get; set; }
	public DateTimeOffset? UsedAt { get; set; }
	public DateTimeOffset? RevokedAt { get; set; }

	public ERevocationReason? RevocationReason { get; set; }


	// Relations
	public Guid UserId { get; set; }
	public User User { get; set; } = null!;

	public Guid? ReplacedTokenId { get; set; }
}
