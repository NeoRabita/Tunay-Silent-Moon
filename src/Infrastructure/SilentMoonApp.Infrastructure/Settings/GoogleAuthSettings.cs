namespace SilentMoonApp.Infrastructure.Settings;

public sealed class GoogleAuthSettings
{
	public const string SectionName = "ExternalAuthSettings:Google";

	public string ClientId { get; set; } = string.Empty;
	public string ClientSecret { get; set; } = string.Empty;
}
