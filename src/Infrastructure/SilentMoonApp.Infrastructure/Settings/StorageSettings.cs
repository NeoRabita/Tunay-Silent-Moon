using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.Infrastructure.Settings;

public class StorageSettings
{
	public const string SectionName = "StorageSettings";

	public EStorageProvider DefaultProvider { get; set; } = EStorageProvider.Local;
	public long MaxFileSizeBytes { get; set; }
	public int MaxUrlExpirationMinutes { get; set; } = 60;
}
