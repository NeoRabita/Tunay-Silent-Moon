using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.Application.DTOs.Storage;

public sealed record StorageFileReference
(
	EStorageProvider StorageProvider,
	string ContainerName,
	string StoredFileName
);
