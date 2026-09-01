using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Domain.Entities;
using System.Runtime.CompilerServices;

namespace SilentMoonApp.Application.Features.Tracks.GetTrackAudioFileUrl;

public class GetTrackAudioFileUrlQueryHandler : IQueryHandler<GetTrackAudioFileUrlQuery, GetTrackAudioFileUrlResult>
{
	private static readonly TimeSpan StreamUrlExpiration = TimeSpan.FromMinutes(10);

	private readonly IUnitOfWork _unitOfWork;
	private readonly IStorageService _storageService;
	private readonly TimeProvider _timeProvider;

	public GetTrackAudioFileUrlQueryHandler(IUnitOfWork unitOfWork,
											IStorageService storageService,
											TimeProvider timeProvider)
	{
		_unitOfWork = unitOfWork;
		_storageService = storageService;
		_timeProvider = timeProvider;
	}


	public async Task<Result<GetTrackAudioFileUrlResult>> Handle(GetTrackAudioFileUrlQuery query,
														   CancellationToken ct = default)
	{
		Track? track = await _unitOfWork.Repository<ITrackReadRepository>()
										.GetTrackDetailAsync(id: query.TrackId,
															 tracking: false,
															 cancellationToken: ct);

		if (track?.AudioFile is null)
			return Result<GetTrackAudioFileUrlResult>.Failure(
				TrackErrors.NotFound());


		StorageFileReference fileReference = new(StorageProvider: track.AudioFile.StorageProvider,
												 ContainerName: track.AudioFile.ContainerName,
												 StoredFileName: track.AudioFile.StoredFileName);

		Result<string> streamUrlResult = await _storageService.GetFileUrlAsync(fileReference,
																			   urlExpiration: StreamUrlExpiration,
																			   cancellationToken: ct);

		if (streamUrlResult.IsFailure)
			return Result<GetTrackAudioFileUrlResult>.Failure(
				TrackErrors.NotFound());


		string contentType = track.AudioFile.ContentType;

		if (string.IsNullOrWhiteSpace(contentType) || contentType == "application/octet-stream")
		{
			contentType = track.AudioFile.UploadedFileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase)
				? "audio/wav"
				: "audio/mpeg";
		}


		return Result<GetTrackAudioFileUrlResult>.Success(
			new GetTrackAudioFileUrlResult(
				TrackId: track.Id,
				CourseId: track.CourseId,
				TrackTitle: track.Title,
				FileName: track.AudioFile.UploadedFileName,
				StreamUrl: streamUrlResult.Value,
				ExpiresAt: _timeProvider.GetUtcNow().Add(StreamUrlExpiration),
				ContentType: contentType,
				FileSizeBytes: track.AudioFile.SizeBytes,
				DurationSec: track.AudioFile.DurationSec));
	}
}
