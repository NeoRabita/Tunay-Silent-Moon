using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.Infrastructure.Storage.Providers;

public abstract class StorageProvider
{
	public abstract EStorageProvider Provider { get; }


	public abstract Task UploadAsync(StorageProviderUploadRequest request,
									 CancellationToken cancellationToken = default);

	public abstract Task DeleteAsync(StorageFileReference fileReference,
									 CancellationToken cancellationToken = default);

	public abstract Task<StorageFileInfo?> GetInfoAsync(StorageFileReference fileReference,
														CancellationToken cancellationToken = default);

	public abstract Task<string> GetFileUrlAsync(StorageFileReference fileReference,
												 TimeSpan? urlExperation = null,
												 CancellationToken cancellationToken = default);

	public abstract Task DownloadAsync(StorageFileReference fileReference,
									   Stream destination,
									   CancellationToken cancellationToken = default);
}
