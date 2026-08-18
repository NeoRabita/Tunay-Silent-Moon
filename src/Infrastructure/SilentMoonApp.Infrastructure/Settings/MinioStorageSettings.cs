namespace SilentMoonApp.Infrastructure.Settings;

public class MinioStorageSettings
{
	public const string SectionName = "StorageSettings:Minio";

	public bool Enabled { get; set; }

	public string Endpoint { get; set; } = string.Empty!;
	public string AccessKey { get; set; } = string.Empty!;
	public string SecretKey { get; set; } = string.Empty!;
	public string BucketName { get; set; } = string.Empty!;
	public bool UseSSL { get; set; }
}