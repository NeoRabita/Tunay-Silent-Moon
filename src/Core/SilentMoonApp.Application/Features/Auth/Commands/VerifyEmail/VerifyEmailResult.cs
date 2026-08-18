namespace SilentMoonApp.Application.Features.Auth.Commands.VerifyEmail;

public sealed record VerifyEmailResult
(
	string AccessToken,
	string RefreshToken,
	string TokenType,
	DateTimeOffset RefreshTokenExpiresAt,
	int AccessTokenExpiresIn,
	VerifyEmailUserResult User
);

public sealed record VerifyEmailUserResult
(
	Guid Id,
	string FirstName,
	string Email,
	bool IsEmailVerified,
	string AvatarUrl,
	DateTimeOffset CreatedAt
);