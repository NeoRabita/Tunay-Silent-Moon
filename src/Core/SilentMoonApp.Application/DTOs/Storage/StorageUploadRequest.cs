namespace SilentMoonApp.Application.DTOs.Storage;

public sealed record StorageUploadRequest
(
	Stream FileStream,
	string ContainerName,
	string UploadedFileName,
	string ContentType,
	long SizeBytes,
	string DirectoryPath
);
