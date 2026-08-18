namespace SilentMoonApp.Application.Features.Auth.Commands.FacebookAuth;

public sealed record FacebookAuthResult
(
	string AccessToken,
	string RefreshToken,
	string TokenType,
	int AccessTokenExpiresIn,
	DateTimeOffset RefreshTokenExpiresAt,
	FacebookAuthUserResult User
);


public sealed record FacebookAuthUserResult
(
	Guid Id,
	string Name,
	string Email,
	bool EmailVerified,
	string? AvatarUrl,
	DateTimeOffset CreatedAt
);

