namespace SilentMoonApp.Infrastructure.Settings;

public class LocalStorageSettings
{
	public const string SectionName = "StorageSettings:Local";

	public bool Enabled { get; set; }

	public string RootPath { get; set; } = string.Empty;
	public string PublicBaseUrl { get; set; } = string.Empty;
}
