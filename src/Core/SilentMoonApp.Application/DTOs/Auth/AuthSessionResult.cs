namespace SilentMoonApp.Application.DTOs.Auth;

public sealed record AuthSessionResult
(
	string AccessToken,
	string RefreshToken,
	Guid RefreshTokenId,
	string TokenType,
	int AccessTokenExpiresIn,
	DateTimeOffset RefreshTokenExpiresAt
);
