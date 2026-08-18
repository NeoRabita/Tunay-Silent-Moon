using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.SharedKernel.Primitives;

namespace SilentMoonApp.Application.Abstractions.Storage;

public interface IStorageService
{
	Task<Result<StorageUploadResult>> UploadFileAsync(StorageUploadRequest request,
													  CancellationToken cancellationToken = default);

	Task<Result<StorageUploadResult>> ReplaceFileAsync(StorageUploadRequest newFile,
													   StorageFileReference existingFile,
													   CancellationToken cancellationToken = default);

	Task DeleteFileAsync(StorageFileReference fileReference,
						 CancellationToken cancellationToken = default);

	Task<bool> FileExistsAsync(StorageFileReference fileReference,
							   CancellationToken cancellationToken = default);

	Task<Result<string>> GetFileUrlAsync(StorageFileReference fileReference,
								 TimeSpan? urlExpiration = null,
								 CancellationToken cancellationToken = default);

	Task<Result> DownloadFileAsync(StorageFileReference fileReference,
						   Stream destinationStream,
						   CancellationToken cancellationToken = default);
}
