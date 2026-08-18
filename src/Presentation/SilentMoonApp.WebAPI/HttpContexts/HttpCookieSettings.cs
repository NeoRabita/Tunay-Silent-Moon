namespace SilentMoonApp.WebAPI.HttpContexts;

public sealed class HttpCookieSettings
{
	public const string SectionName = "HttpCookieSettings";

	public string Name { get; init; } = "refreshToken";
	public string Path { get; init; } = "/api/auth";
	public SameSiteMode SameSite { get; init; } = SameSiteMode.Strict;
	
	//public string? Domain { get; init; }
}
