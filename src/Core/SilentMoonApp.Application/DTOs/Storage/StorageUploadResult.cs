using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.Application.DTOs.Storage;

public sealed record StorageUploadResult
(
	EStorageProvider Provider,
	string ContainerName,
	string StoredFileName,
	string UploadedFileName,
	string Extension,
	string ContentType,
	long SizeBytes
);
