using Minio;
using Minio.Exceptions;
using Minio.DataModel.Args;
using SilentMoonApp.Domain.Enums;
using Microsoft.Extensions.Logging;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Exceptions.Storage;


namespace SilentMoonApp.Infrastructure.Storage.Providers;

public class MinioStorageProvider : StorageProvider
{
	private static readonly HttpClient _httpClient = new();
	private const int StreamBufferSize = 81920; // 80 KB

	private readonly IMinioClient _minioClient;
	private readonly ILogger<MinioStorageProvider> _logger;

	public MinioStorageProvider(IMinioClient minioClient,
								ILogger<MinioStorageProvider> logger)
	{
		_minioClient = minioClient;
		_logger = logger;
	}




	public override EStorageProvider Provider => EStorageProvider.Minio;


	public override async Task UploadAsync(StorageProviderUploadRequest request,
									 CancellationToken ct = default)
	{
		try
		{
			await EnsureBucketExistsAsync(bucketName: request.FileReference.ContainerName,
										  ct: ct);

			var args = new PutObjectArgs().WithBucket(request.FileReference.ContainerName)
										  .WithObject(request.FileReference.StoredFileName)
										  .WithStreamData(request.FileStream)
										  .WithObjectSize(request.SizeBytes)
										  .WithContentType(request.ContentType);

			await _minioClient.PutObjectAsync(args: args,
											  cancellationToken: ct);
		}

		catch (MinioException exception)
		{
			_logger.LogError(exception, "MinIO upload failed. Bucket: {Bucket}, Object: {Object}",
							 request.FileReference.ContainerName,
							 request.FileReference.StoredFileName);

			throw new StorageOperationException(message: "MinIO upload operation failed.",
												innerException: exception);
		}
	}



	public override async Task DeleteAsync(StorageFileReference fileReference,
										   CancellationToken ct = default)
	{
		try
		{
			var args = new RemoveObjectArgs().WithBucket(fileReference.ContainerName)
											 .WithObject(fileReference.StoredFileName);

			await _minioClient.RemoveObjectAsync(args: args,
												 cancellationToken: ct);
		}

		catch (ObjectNotFoundException)
		{
			return;
		}

		catch (MinioException exception)
		{
			_logger.LogError(exception, "MinIO delete failed. Bucket: {Bucket}, Object: {Object}",
							 fileReference.ContainerName,
							 fileReference.StoredFileName);

			throw new StorageOperationException(message: "MinIO delete operation failed.",
												innerException: exception);
		}
	}



	public override async Task<StorageFileInfo?> GetInfoAsync(StorageFileReference fileReference,
															  CancellationToken ct = default)
	{
		try
		{
			var args = new StatObjectArgs().WithBucket(fileReference.ContainerName)
										   .WithObject(fileReference.StoredFileName);

			var objectStat = await _minioClient.StatObjectAsync(args: args,
																cancellationToken: ct);

			return new StorageFileInfo
			(
				StorageFile: fileReference,
				ContentType: string.IsNullOrWhiteSpace(objectStat.ContentType) ? "application/octet-stream"
																			   : objectStat.ContentType,
				SizeBytes: objectStat.Size
			);

		}

		catch (ObjectNotFoundException)
		{
			return null;
		}

		catch (MinioException exception)
		{
			_logger.LogError(exception, "MinIO get info failed. Bucket: {Bucket}, Object: {Object}",
							 fileReference.ContainerName,
							 fileReference.StoredFileName);

			throw new StorageOperationException(message: "MinIO get info operation failed.",
												innerException: exception);
		}

	}



	public override async Task<string> GetFileUrlAsync(StorageFileReference fileReference,
												TimeSpan? urlExpiration = null,
												CancellationToken ct = default)
	{
		ct.ThrowIfCancellationRequested();


		try
		{
			int expirationSeconds = checked((int)(urlExpiration?.TotalSeconds ?? 3600));

			var args = new PresignedGetObjectArgs().WithBucket(fileReference.ContainerName)
												   .WithObject(fileReference.StoredFileName)
												   .WithExpiry(expirationSeconds);

			return await _minioClient.PresignedGetObjectAsync(args: args);
		}

		catch (MinioException exception)
		{
			_logger.LogError(exception, "MinIO get file URL failed. Bucket: {Bucket}, Object: {Object}",
							 fileReference.ContainerName,
							 fileReference.StoredFileName);

			throw new StorageOperationException(message: "MinIO get file URL operation failed.",
												innerException: exception);
		}
	}



	public override async Task DownloadAsync(StorageFileReference fileReference,
											 Stream destinationStream,
											 CancellationToken ct = default)
	{
		try
		{
			var args = new GetObjectArgs().WithBucket(fileReference.ContainerName)
										  .WithObject(fileReference.StoredFileName)
										  .WithCallbackStream(async (sourceStream, callbackCancellationToken) =>
																	{
																		await sourceStream.CopyToAsync(
																			destinationStream,
																			StreamBufferSize,
																			callbackCancellationToken);
																	});

			await _minioClient.GetObjectAsync(args: args,
											  cancellationToken: ct);
		}

		catch (ObjectNotFoundException)
		{
			throw new StorageNotFoundException(containerName: fileReference.ContainerName,
												   storedFileName: fileReference.StoredFileName);
		}

		catch (MinioException exception)
		{
			_logger.LogError(exception, "MinIO download failed. Bucket: {Bucket}, Object: {Object}",
							 fileReference.ContainerName,
							 fileReference.StoredFileName);

			throw new StorageOperationException(message: "MinIO download operation failed.",
												innerException: exception);
		}

	}



	public override async Task<StorageStreamResult> OpenReadStreamAsync(StorageFileReference fileReference,
																		string? rangeHeader = null,
																		TimeSpan? urlExpiration = null,
																		CancellationToken ct = default)
	{
		try
		{
			string url = await GetFileUrlAsync(fileReference: fileReference,
											   urlExpiration: urlExpiration,
											   ct: ct);

			using HttpRequestMessage requestMessage = new(method: HttpMethod.Get,
														  requestUri: url);

			if (!string.IsNullOrWhiteSpace(rangeHeader))
				requestMessage.Headers.Add(name: "Range",
											value: rangeHeader);


			HttpResponseMessage responseMessage = await _httpClient.SendAsync(request: requestMessage,
																			  completionOption: HttpCompletionOption.ResponseHeadersRead,
																			  cancellationToken: ct);

			if (!responseMessage.IsSuccessStatusCode)
			{
				responseMessage.Dispose();
				throw new StorageOperationException(message: $"Failed to open read stream. HTTP Status Code: {responseMessage.StatusCode}");
			}


			Stream audioStream = await responseMessage.Content.ReadAsStreamAsync(ct);

			string contentType = responseMessage.Content.Headers.ContentType?.ToString()
							  ?? "application/octet-stream";

			return new StorageStreamResult(stream: audioStream,
										   contentType: contentType,
										   contentLength: responseMessage.Content.Headers.ContentLength,
										   contentRange: responseMessage.Content.Headers.ContentRange?.ToString(),
										   acceptRanges: responseMessage.Content.Headers.Any(),
										   statusCode: (int)responseMessage.StatusCode,
										   lease: responseMessage);
		}

		catch (ObjectNotFoundException)
		{
			throw new StorageNotFoundException(containerName: fileReference.ContainerName,
											   storedFileName: fileReference.StoredFileName);
		}

		catch (MinioException exception)
		{
			_logger.LogError(exception, "MinIO open read stream failed. Bucket: {Bucket}, Object: {Object}",
							 fileReference.ContainerName,
							 fileReference.StoredFileName);
			throw new StorageOperationException(message: "MinIO open read stream operation failed.",
												innerException: exception);
		}
	}



	// Helpers

	private async Task EnsureBucketExistsAsync(string bucketName,
											   CancellationToken ct)
	{
		var bucketExistsArgs = new BucketExistsArgs().WithBucket(bucketName);

		bool bucketExists = await _minioClient.BucketExistsAsync(args: bucketExistsArgs,
																cancellationToken: ct);

		if (bucketExists)
			return;

		var makeBucketArgs = new MakeBucketArgs().WithBucket(bucketName);

		await _minioClient.MakeBucketAsync(args: makeBucketArgs,
										   cancellationToken: ct);

		_logger.LogInformation(message: "MinIO Bucket Created. Bucket: {Bucket}",
							   args: bucketName);
	}

}
