namespace SilentMoonApp.Application.DTOs.Auth;

public sealed record GeneratedAccessTokenResult
(
	string AccessToken,
	DateTimeOffset ExpiresAt,
	int AccessTokenExpiryMinutes
);
