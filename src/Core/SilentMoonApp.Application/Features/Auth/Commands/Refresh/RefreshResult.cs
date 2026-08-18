namespace SilentMoonApp.Application.Features.Auth.Commands.Refresh;

public sealed record RefreshResult
(
	string AccessToken,
	string RefreshToken,
	string TokenType,
	DateTimeOffset RefreshTokenExpiresAt,
	int AccessTokenExpiresIn,
	RefreshUserResult User
);

public sealed record RefreshUserResult
(
	Guid Id,
	string FirstName,
	string Email,
	bool IsEmailVerified,
	string AvatarUrl,
	DateTimeOffset CreatedAt
);