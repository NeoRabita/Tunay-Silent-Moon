using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.Application.DTOs.Auth;

public sealed record OtpCacheVerificationResult
(
	EOtpVerificationStatus OtpVerificationStatus,
	int RemainingAttempts
);
