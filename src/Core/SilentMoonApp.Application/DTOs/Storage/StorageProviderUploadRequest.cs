namespace SilentMoonApp.Application.DTOs.Storage;

public sealed record StorageProviderUploadRequest
(
	StorageFileReference FileReference,
	Stream FileStream,
	string ContentType,
	long SizeBytes
);
