using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Exceptions.Storage;
using SilentMoonApp.Domain.Enums;
using SilentMoonApp.Infrastructure.Settings;


namespace SilentMoonApp.Infrastructure.Storage.Providers;

public class LocalStorageProvider : StorageProvider
{
	private const int StreamBufferSize = 81920;

	private readonly string _rootPath;
	private readonly string _publicBaseUrl;
	private readonly ILogger<LocalStorageProvider> _logger;


	public LocalStorageProvider(IOptions<LocalStorageSettings> options,
								ILogger<LocalStorageProvider> logger)
	{
		LocalStorageSettings settings = options.Value;

		_rootPath = Path.GetFullPath(settings.RootPath);
		_publicBaseUrl = settings.PublicBaseUrl.TrimEnd('/');
		_logger = logger;
	}


	public override EStorageProvider Provider => EStorageProvider.Local;



	public override async Task UploadAsync(StorageProviderUploadRequest request, CancellationToken cancellationToken = default)
	{
		string physicalPath = GetPhysicalPath(request.FileReference);

		string? directoryPath = Path.GetDirectoryName(physicalPath);


		if (string.IsNullOrWhiteSpace(directoryPath))
			throw new StorageOperationException($"Failed to determine the directory path for '{physicalPath}'.");


		try
		{
			Directory.CreateDirectory(directoryPath);

			await using FileStream destinationStream = new(path: physicalPath,
														   mode: FileMode.Create,
														   access: FileAccess.Write,
														   share: FileShare.None,
														   bufferSize: StreamBufferSize,
														   options: FileOptions.Asynchronous | FileOptions.SequentialScan);

			await request.FileStream.CopyToAsync(destination: destinationStream,
												 bufferSize: StreamBufferSize,
												 cancellationToken: cancellationToken);
		}

		catch (Exception ex)
			when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
		{
			throw new StorageOperationException(message: $"An Error occurred while Uploading The File '{physicalPath}'.",
												innerException: ex);
		}
	}


	public override Task DeleteAsync(StorageFileReference fileReference,
									 CancellationToken ct = default)
	{
		ct.ThrowIfCancellationRequested();


		string physicalPath = GetPhysicalPath(fileReference);

		try
		{
			File.Delete(physicalPath);

			return Task.CompletedTask;
		}

		catch (Exception ex)
			when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
		{
			throw new StorageOperationException(message: $"An Error occurred while Deleting The File '{physicalPath}'.",
												innerException: ex);
		}
	}



	public override Task<StorageFileInfo?> GetInfoAsync(StorageFileReference fileReference, CancellationToken ct = default)
	{
		ct.ThrowIfCancellationRequested();


		string physicalPath = GetPhysicalPath(fileReference);

		var fileInfo = new FileInfo(physicalPath);

		try
		{
			if (!fileInfo.Exists)
				return Task.FromResult<StorageFileInfo?>(null);


			string contentType = fileInfo.Extension.ToLowerInvariant() switch
			{
				".jpg" or ".jpeg" => "image/jpeg",
				".png" => "image/png",
				".webp" => "image/webp",
				".gif" => "image/gif",
				".mp3" => "audio/mpeg",
				".wav" => "audio/wav",
				_ => "application/octet-stream"
			};


			var result = new StorageFileInfo
			(
				StorageFile: fileReference,
				ContentType: contentType,
				SizeBytes: fileInfo.Length
			);


			return Task.FromResult<StorageFileInfo?>(result);
		}

		catch (FileNotFoundException)
		{
			return Task.FromResult<StorageFileInfo?>(null);
		}

		catch (DirectoryNotFoundException)
		{
			return Task.FromResult<StorageFileInfo?>(null);
		}

		catch (Exception exception)
			when (exception is IOException or
							   UnauthorizedAccessException or
							   NotSupportedException)
		{
			throw new StorageOperationException(message: $"An error occurred while reading file information for '{physicalPath}'.",
												innerException: exception);
		}
	}


	public override Task<string> GetFileUrlAsync(StorageFileReference fileReference,
												 TimeSpan? urlExpiration = null,
												 CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();


		IEnumerable<string> urlSegments = new[]
		{
			fileReference.ContainerName,
		}
		.Concat(fileReference.StoredFileName.Split('/', StringSplitOptions.RemoveEmptyEntries));


		string relativeUrl = string.Join('/', urlSegments.Select(Uri.EscapeDataString));


		return Task.FromResult($"{_publicBaseUrl}/{relativeUrl}");
	}


	public override async Task DownloadAsync(StorageFileReference fileReference,
									   Stream destinationStream,
									   CancellationToken ct = default)
	{
		string physicalPath = GetPhysicalPath(fileReference);

		//if (!File.Exists(physicalPath))
		//	throw new("The specified file does not exist.", physicalPath);

		try
		{
			await using FileStream sourceStream = new(path: physicalPath,
													  mode: FileMode.Open,
													  access: FileAccess.Read,
													  share: FileShare.Read,
													  bufferSize: StreamBufferSize,
													  useAsync: true);

			await sourceStream.CopyToAsync(destination: destinationStream,
										   bufferSize: StreamBufferSize,
										   cancellationToken: ct);
		}

		catch (FileNotFoundException)
		{
			throw new StorageNotFoundException(containerName: fileReference.ContainerName,
									  storedFileName: fileReference.StoredFileName);
		}

		catch (DirectoryNotFoundException)
		{
			throw new StorageNotFoundException(containerName: fileReference.ContainerName,
									  storedFileName: fileReference.StoredFileName);
		}

		catch (Exception exception)
			when (exception is IOException or UnauthorizedAccessException or NotSupportedException)
		{
			throw new StorageOperationException(message: $"An error occurred while downloading the file from '{physicalPath}'.",
												innerException: exception);
		}
	}




	// Helpers

	private string GetPhysicalPath(StorageFileReference fileReference)
	{
		string localObjectPath = fileReference.StoredFileName.Replace('/', Path.DirectorySeparatorChar);

		string physicalPath = Path.GetFullPath(
			Path.Combine(_rootPath,
						 fileReference.ContainerName,
						 localObjectPath));


		string rootWithSeparator = _rootPath.EndsWith(Path.DirectorySeparatorChar)
								 ? _rootPath
								 : _rootPath + Path.DirectorySeparatorChar;

		StringComparison comparison = OperatingSystem.IsWindows()
									? StringComparison.OrdinalIgnoreCase
									: StringComparison.Ordinal;


		if (!physicalPath.StartsWith(rootWithSeparator, comparison))
			throw new ArgumentException("local Storage Path Root Directory Traversal detected", nameof(fileReference));


		return physicalPath;
	}
}
