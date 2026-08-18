namespace SilentMoonApp.Application.Features.Auth.Commands.Login;


public sealed record LoginResult
(
	string AccessToken,
	string RawRefreshToken,
	string TokenType,
	int AccessTokenExpiresIn,
	DateTimeOffset RefreshTokenExpiresAt,
	LoginUserResult User
);

public sealed record LoginUserResult
(
	Guid Id,
	string FirstName,
	string Email,
	bool IsEmailVerified,
	string? AvatarUrl,
	DateTimeOffset CreatedAt
);
