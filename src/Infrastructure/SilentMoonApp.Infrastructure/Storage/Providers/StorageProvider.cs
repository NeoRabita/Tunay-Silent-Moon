using SilentMoonApp.Domain.Enums;
using SilentMoonApp.Application.DTOs.Storage;


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
												 TimeSpan? urlExpiration = null,
												 CancellationToken cancellationToken = default);

	public abstract Task DownloadAsync(StorageFileReference fileReference,
									   Stream destination,
									   CancellationToken cancellationToken = default);

	public virtual Task<StorageStreamResult> OpenReadStreamAsync(StorageFileReference fileReference,
																 string? rangeHeader = null,
																 TimeSpan? urlExpiration = null,
																 CancellationToken cancellationToken = default) 
		
		=> throw new NotImplementedException("OpenReadStreamAsync is not implemented for this storage provider.");

}
