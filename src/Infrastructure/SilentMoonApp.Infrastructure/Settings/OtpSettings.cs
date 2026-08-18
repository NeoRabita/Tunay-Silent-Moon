namespace SilentMoonApp.Application.Settings;

public sealed class OtpSettings
{
	public const string SectionName = "OtpSettings";

	public int Length { get; init; } = 6;
	public int ExpirationMinutes { get; init; } = 10;
	public int MaxFailedAttempts { get; init; } = 5;

	public string OtpHmacKey { get; init; } = string.Empty;
}
