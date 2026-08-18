using SilentMoonApp.Domain.Entities.Identity;

namespace SilentMoonApp.Application.DTOs.Auth;

public sealed record GeneratedRefreshTokenResult
(
	string RawRefreshToken,
	RefreshToken RefreshToken,
	int RefreshTokenExpiryDays
);
