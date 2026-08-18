using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.Application.Abstractions.Caching;

public interface IOtpCacheService
{
	Task StoreOtpAsync(Guid userId,
					   EOtpPurpose otpPurpose,
					   string rawCode,
					   DateTimeOffset expiresAt,
					   CancellationToken ct = default);

	Task<OtpCacheVerificationResult> VerifyOtpAsync(Guid userId,
													EOtpPurpose otpPurpose,
													string rawCode,
													CancellationToken ct = default);

	Task<bool> RemoveOtpAsync(Guid userId,
						      EOtpPurpose otpPurpose,
						      CancellationToken ct = default);
}
