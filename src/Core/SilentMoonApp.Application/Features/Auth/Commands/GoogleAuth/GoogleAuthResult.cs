namespace SilentMoonApp.Application.Features.Auth.Commands.GoogleAuth;

public sealed record GoogleAuthResult
(
	string AccessToken,
	string RefreshToken,
	string TokenType,
	int AccessTokenExpiresIn,
	DateTimeOffset RefreshTokenExpiresAt,
	GoogleAuthUserResult User
);


public sealed record GoogleAuthUserResult
(
	Guid Id,
	string Name,
	string Email,
	bool EmailVerified,
	string? AvatarUrl,
	DateTimeOffset CreatedAt
);
