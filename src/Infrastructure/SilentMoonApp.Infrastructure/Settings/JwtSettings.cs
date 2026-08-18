namespace SilentMoonApp.Infrastructure.Settings;

public sealed class JwtSettings
{
	public const string SectionName = "JwtSettings";

	public string Issuer { get; init; } = string.Empty;
	public string Audience { get; init; } = string.Empty;
	public string SecretKey { get; init; } = string.Empty;

	public int RefreshTokenExpirationDays { get; init; } = 7;
	public int AccessTokenExpirationMinutes { get; init; } = 15;

	public string RefreshTokenHmacKey { get; init; } = string.Empty;
	public string RefreshTokenCookieName { get; init; } = "refreshToken";
}
