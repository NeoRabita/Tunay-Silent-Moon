using SilentMoonApp.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using SilentMoonApp.Infrastructure.Settings;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Exceptions.Storage;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Infrastructure.Storage.Providers;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.Application.Errors;


namespace SilentMoonApp.Infrastructure.Storage;

public class StorageService : IStorageService
{
	public const int MaxUploadedFileNameLength = 255;
	private const int MaxObjectNameLength = 1024;

	private static readonly IReadOnlyDictionary<string, ContentTypeDTO> AllowedFileTypes = new Dictionary<string, ContentTypeDTO>(StringComparer.OrdinalIgnoreCase)
	{
		[".jpg"] = new("image/jpeg",
					  ["image/jpeg"]),

		[".jpeg"] = new("image/jpeg",
					   ["image/jpeg"]),

		[".png"] = new("image/png",
					  ["image/png"]),

		[".webp"] = new("image/webp",
					   ["image/webp"]),

		[".gif"] = new("image/gif",
					  ["image/gif"]),

		[".mp3"] = new("audio/mpeg",
					  ["audio/mpeg"]),

		[".wav"] = new("audio/wav",
					  ["audio/wav", "audio/x-wav"])
	};


	private readonly IReadOnlyDictionary<EStorageProvider, StorageProvider> _providers;
	private readonly StorageSettings _settings;
	private readonly TimeProvider _timeProvider;
	private readonly ILogger<StorageService> _logger;


	public StorageService(IEnumerable<StorageProvider> providers,
						  IOptions<StorageSettings> options,
						  TimeProvider timeProvider,
						  ILogger<StorageService> logger)
	{
		StorageProvider[] registeredProviders = providers.ToArray();


		if (registeredProviders.Length == 0)
			throw new InvalidOperationException("No storage providers registered. At least one storage provider must be registered.");

		IGrouping<EStorageProvider, StorageProvider>? duplicateProvider = registeredProviders.GroupBy(p => p.Provider)
																							.FirstOrDefault(group => group.Count() > 1);

		if (duplicateProvider is not null)
			throw new InvalidOperationException($"Duplicate storage provider registered: {duplicateProvider.Key}. Each storage provider must be unique.");


		_providers = registeredProviders.ToDictionary(provider => provider.Provider);
		_settings = options.Value;
		_timeProvider = timeProvider;
		_logger = logger;


		if (!_providers.ContainsKey(_settings.DefaultProvider))
			throw new InvalidOperationException("Default storage provider is not configured. Please configure a default storage provider in the settings.");
	}



	public async Task<Result<StorageUploadResult>> UploadFileAsync(StorageUploadRequest request,
																   CancellationToken ct = default)
	{
		Result<(string extension, string contentType)> validateFile = await ValidateUploadAsync(request: request,
																								 ct: ct);

		if (validateFile.IsFailure)
			return Result<StorageUploadResult>.Failure(
				validateFile.Error);

		(string extension, string contentType) = validateFile.Value;


		StorageProvider storageProvider = GetProvider(_settings.DefaultProvider);


		string objectName = GenerateObjectName(directory: request.DirectoryPath,
											   extension: extension);

		StorageFileReference fileReference = new
		(
			StorageProvider: storageProvider.Provider,
			ContainerName: request.ContainerName,
			StoredFileName: objectName
		);


		await storageProvider.UploadAsync(request: new StorageProviderUploadRequest(FileReference: fileReference,
																					FileStream: request.FileStream,
																					ContentType: contentType,
																					SizeBytes: request.SizeBytes),
										  cancellationToken: ct);


		_logger.LogInformation("Storage file uploaded. Provider: {Provider}, Container: {Container}, Object: {Object}, Size: {Size}",
							   storageProvider.Provider,
							   request.ContainerName,
							   objectName, request.SizeBytes);


		return Result<StorageUploadResult>.Success(
			new StorageUploadResult
			(
				Provider: storageProvider.Provider,
				ContainerName: request.ContainerName,
				StoredFileName: objectName,
				UploadedFileName: request.UploadedFileName,
				Extension: extension,
				ContentType: contentType,
				SizeBytes: request.SizeBytes
			)
		);
	}


	public async Task<Result<StorageUploadResult>> ReplaceFileAsync(StorageUploadRequest newFile,
																	StorageFileReference existingFile,
																	CancellationToken ct = default)
	{
		ValidateFileReference(existingFile);


		Result<StorageUploadResult> uploadedFile = await UploadFileAsync(request: newFile,
																		 ct: ct);

		if (uploadedFile.IsFailure)
			return Result<StorageUploadResult>.Failure(uploadedFile.Error);


		var uploadedFileReference = new StorageFileReference(StorageProvider: uploadedFile.Value.Provider,
															 ContainerName: uploadedFile.Value.ContainerName,
															 StoredFileName: uploadedFile.Value.StoredFileName);


		try
		{
			StorageProvider oldProvider = GetProvider(existingFile.StorageProvider);

			await oldProvider.DeleteAsync(fileReference: existingFile, cancellationToken: ct);


			return Result<StorageUploadResult>.Success(uploadedFile.Value);
		}

		catch (Exception deleteException)
			when (deleteException is not OperationCanceledException)
		{
			try
			{
				StorageProvider newProvider = GetProvider(uploadedFile.Value.Provider);

				await newProvider.DeleteAsync(fileReference: uploadedFileReference, cancellationToken: ct);
			}

			catch (Exception newProviderException)
			{
				_logger.LogError(exception: newProviderException,
								 message: "Error occurred while handling the new provider. Provider: {Provider}, Container: {Container}, Object: {Object}",
								 args: [uploadedFile.Value.Provider, uploadedFile.Value.ContainerName, uploadedFile.Value.StoredFileName]);
			}

			throw new StorageOperationException(message: "New File uploaded. But Existing File could not be deleted.",
												innerException: deleteException);
		}
	}


	public async Task DeleteFileAsync(StorageFileReference fileReference,
									  CancellationToken ct = default)
	{
		ValidateFileReference(fileReference);


		StorageProvider storageProvider = GetProvider(fileReference.StorageProvider);


		await storageProvider.DeleteAsync(fileReference: fileReference,
										  cancellationToken: ct);


		_logger.LogInformation("Storage file deleted. Provider: {Provider}, Container: {Container}, Object: {Object}",
							   fileReference.StorageProvider,
							   fileReference.ContainerName,
							   fileReference.StoredFileName);
	}


	public Task<StorageFileInfo?> GetFileInfoAsync(StorageFileReference fileReference,
												   CancellationToken ct = default)
	{
		ValidateFileReference(fileReference);


		StorageProvider storageProvider = GetProvider(fileReference.StorageProvider);

		return storageProvider.GetInfoAsync(fileReference: fileReference,
									 cancellationToken: ct);
	}


	public async Task<bool> FileExistsAsync(StorageFileReference fileReference,
											CancellationToken ct = default)
	{
		StorageFileInfo? fileInfo = await GetFileInfoAsync(fileReference: fileReference,
														   ct: ct);

		return fileInfo is not null;
	}


	public async Task<Result<string>> GetFileUrlAsync(StorageFileReference fileReference,
										TimeSpan? urlExperation = null,
										CancellationToken ct = default)
	{
		ValidateFileReference(fileReference);


		if (urlExperation is not null)
		{
			if (urlExperation <= TimeSpan.Zero)
				throw new ArgumentOutOfRangeException(paramName: nameof(urlExperation),
													  message: "Url-Experation must be greater than zero.");


			TimeSpan maximumExpiration = TimeSpan.FromMinutes(_settings.MaxUrlExpirationMinutes);

			if (urlExperation > maximumExpiration)
				throw new ArgumentOutOfRangeException(paramName: nameof(urlExperation),
													  message: $"Url-Experation must be less than or equal to the maximum allowed expiration of {maximumExpiration}.");
		}


		StorageProvider storageProvider = GetProvider(fileReference.StorageProvider);

		StorageFileInfo? fileInfo = await storageProvider.GetInfoAsync(fileReference: fileReference,
																	   cancellationToken: ct);

		if (fileInfo is null)
			return Result<string>.Failure(
				StorageErrors.FileNotFound());


		return Result<string>.Success(
			await storageProvider.GetFileUrlAsync(fileReference: fileReference,
												  urlExperation: urlExperation,
												  cancellationToken: ct)
		);
	}


	public async Task<Result> DownloadFileAsync(StorageFileReference fileReference,
										  Stream destinationStream,
										  CancellationToken ct = default)
	{
		ValidateFileReference(fileReference);

		ArgumentNullException.ThrowIfNull(destinationStream);

		if (!destinationStream.CanWrite)
			throw new ArgumentException("Destination stream must be writable.", nameof(destinationStream));


		StorageProvider storageProvider = GetProvider(fileReference.StorageProvider);


		try
		{
			await storageProvider.DownloadAsync(
				fileReference: fileReference,
				destination: destinationStream,
				cancellationToken: ct);


			return Result.Success();
		}
		catch (StorageNotFoundException)
		{
			return Result.Failure(
				StorageErrors.FileNotFound());
		}
	}



	// Helpers

	private async Task<Result<(string, string)>> ValidateUploadAsync(StorageUploadRequest request,
															CancellationToken ct)
	{
		ArgumentNullException.ThrowIfNull(request);
		ArgumentNullException.ThrowIfNull(request.FileStream);


		if (!request.FileStream.CanRead)
			throw new ArgumentException("File stream must be readable.", nameof(request.FileStream));

		if (!request.FileStream.CanSeek)
			throw new ArgumentException("File stream must be seekable.", nameof(request.FileStream));


		if (request.SizeBytes <= 0)
			return Result<(string, string)>.Failure(
				StorageErrors.InvalidFile());

		if (request.SizeBytes > _settings.MaxFileSizeBytes)
			return Result<(string, string)>.Failure(StorageErrors.FileTooLarge(_settings.MaxFileSizeBytes));

		if (request.FileStream.Length != request.SizeBytes)
			return Result<(string, string)>.Failure(
				StorageErrors.InvalidFile());


		if (string.IsNullOrWhiteSpace(request.UploadedFileName))
			return Result<(string, string)>.Failure(
				StorageErrors.InvalidFileName());

		if (request.UploadedFileName.Length > MaxUploadedFileNameLength)
			return Result<(string, string)>.Failure(
				StorageErrors.InvalidFileName());


		if (request.UploadedFileName.Contains("..") ||
			request.UploadedFileName.Contains("/") ||
			request.UploadedFileName.Contains("\\"))

			return Result<(string, string)>.Failure(
				StorageErrors.InvalidFileName());


		if (string.IsNullOrWhiteSpace(request.ContainerName) ||
			request.ContainerName.Contains('/') ||
			request.ContainerName.Contains('\\') ||
			request.ContainerName.Contains(".."))

			throw new ArgumentException("Container name must be provided and must not contain directory traversal characters.", nameof(request.ContainerName));


		if (string.IsNullOrWhiteSpace(request.DirectoryPath) ||
			request.DirectoryPath.Contains('\\') ||
			request.DirectoryPath.Split('/')
								 .Any(segment => string.IsNullOrWhiteSpace(segment) ||
												 segment is "." or ".."))

			throw new ArgumentException("Directory path must be provided and must not contain directory traversal characters.", nameof(request.DirectoryPath));



		string extension = Path.GetExtension(request.UploadedFileName).ToLowerInvariant();


		if (string.IsNullOrWhiteSpace(extension) || !AllowedFileTypes.TryGetValue(key: extension,
																				  value: out ContentTypeDTO? contentTypeDTO))

			return Result<(string, string)>.Failure(
				StorageErrors.UnSupportedFileType());


		if (string.IsNullOrWhiteSpace(request.ContentType))
			return Result<(string, string)>.Failure(
				StorageErrors.InvalidContentType());



		string declaredContentType = request.ContentType.Split(';', 2)[0]
														.Trim()
														.ToLowerInvariant();

		if (!contentTypeDTO.AllowedContentTypes.Contains(declaredContentType,
														 StringComparer.OrdinalIgnoreCase))

			return Result<(string, string)>.Failure(
				StorageErrors.InvalidContentType());


		string? detectedContentType = await DetectContentTypeAsync(fileStream: request.FileStream,
																   ct: ct);

		if (detectedContentType is null)
			return Result<(string, string)>.Failure(
				StorageErrors.UnSupportedFileType());


		if (!string.Equals(detectedContentType, contentTypeDTO.PreferredContentType, StringComparison.OrdinalIgnoreCase))
			return Result<(string, string)>.Failure(
				StorageErrors.InvalidContentType());


		request.FileStream.Position = 0;


		return Result<(string, string)>.Success(
			(
			extension,
			detectedContentType
			)
		);
	}


	private static void ValidateFileReference(StorageFileReference fileReference)
	{
		ArgumentNullException.ThrowIfNull(fileReference);

		if (!Enum.IsDefined(fileReference.StorageProvider))
			throw new ArgumentException("The Storage Provider is Incorrect.");


		if (string.IsNullOrWhiteSpace(fileReference.ContainerName) ||
		   fileReference.ContainerName.Contains('/') ||
		   fileReference.ContainerName.Contains('\\') ||
		   fileReference.ContainerName.Contains(".."))

			throw new ArgumentException("Storage ContainerName is Incorrect.");


		if (string.IsNullOrWhiteSpace(fileReference.StoredFileName) ||
			fileReference.StoredFileName.Length > MaxObjectNameLength ||
			fileReference.StoredFileName.StartsWith('/') ||
			fileReference.StoredFileName.Contains('\\') ||
			fileReference.StoredFileName.Split('/')
										.Any(segment => string.IsNullOrWhiteSpace(segment) ||
														segment is "." or ".."))

			throw new ArgumentException("Storage Stored-FIleName is Incorrect.");
	}


	private static async Task<string?> DetectContentTypeAsync(Stream fileStream,
															CancellationToken ct)
	{
		fileStream.Position = 0;

		byte[] header = new byte[12];

		int read = await fileStream.ReadAsync(header.AsMemory(), ct);

		fileStream.Position = 0;


		return DetectContentTypeBase(header, read);
	}


	private static string? DetectContentTypeBase(byte[] header,
												int read)
	{
		ReadOnlySpan<byte> bytes = header.AsSpan(0, read);


		if (bytes.Length >= 3 &&
			bytes[0] == 0xFF &&
			bytes[1] == 0xD8 &&
			bytes[2] == 0xFF)

			return "image/jpeg";


		if (bytes.Length >= 8 &&
			bytes[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))

			return "image/png";


		if (bytes.Length >= 12 &&
			bytes[..4].SequenceEqual("RIFF"u8) &&
			bytes.Slice(8, 4).SequenceEqual("WEBP"u8))

			return "image/webp";


		if (bytes.Length >= 6 &&
		   (bytes[..6].SequenceEqual(new byte[] { 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }) ||
			bytes[..6].SequenceEqual("GIF87a"u8) ||
			bytes[..6].SequenceEqual("GIF89a"u8)))

			return "image/gif";


		if (bytes.Length >= 12 &&
			bytes[..4].SequenceEqual("RIFF"u8) &&
			bytes.Slice(8, 4).SequenceEqual("WAVE"u8))

			return "audio/wav";


		if (bytes.Length >= 3 &&
			bytes[..3].SequenceEqual("ID3"u8))

			return "audio/mpeg";


		if (bytes.Length >= 2 &&
			bytes[0] == 0xFF &&
			(bytes[1] & 0xE0) == 0xE0)

			return "audio/mpeg";


		return null;
	}


	private string GenerateObjectName(string directory,
									  string extension)
	{
		DateTimeOffset nowUtc = _timeProvider.GetUtcNow();

		string normalizedDirectory = directory.Trim('/');

		string generatedFileName = $"{Guid.NewGuid():N}{extension}";


		return string.Join('/',
						   normalizedDirectory,
						   nowUtc.Year.ToString("0000"),
						   nowUtc.Month.ToString("00"),
						   generatedFileName);
	}


	private StorageProvider GetProvider(EStorageProvider provider)

		 => _providers.TryGetValue(provider, out StorageProvider? storageProvider)
				? storageProvider
				: throw new InvalidOperationException($"Storage provider is not active or not registered in DI: {provider}");
}
