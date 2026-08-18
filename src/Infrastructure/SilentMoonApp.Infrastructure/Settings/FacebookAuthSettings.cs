namespace SilentMoonApp.Infrastructure.Settings;

public class FacebookAuthSettings
{
	public const string SectionName = "ExternalAuthSettings:Facebook";

	public string AppId { get; init; } = string.Empty;
	public string AppSecret { get; init; } = string.Empty;
	public string MetaDataAddress { get; init; } = "https://www.facebook.com/.well-known/openid-configuration";

	//public string GraphApiVersion { get; init; } = "v26.0";
}
