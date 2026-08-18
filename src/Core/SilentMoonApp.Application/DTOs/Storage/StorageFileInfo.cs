namespace SilentMoonApp.Application.DTOs.Storage;

public sealed record StorageFileInfo
(
	StorageFileReference StorageFile,
	string ContentType,
	long SizeBytes
);
