namespace SilentMoonApp.Application.DTOs.Auth;

public sealed record GeneratedOtpResult
(
	string RawCode,
	DateTimeOffset ExpiresAt,
	int LifeTimeMinutes
);
