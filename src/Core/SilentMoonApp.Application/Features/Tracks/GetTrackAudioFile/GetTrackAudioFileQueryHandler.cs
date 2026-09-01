using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Domain.Entities;


namespace SilentMoonApp.Application.Features.Tracks.GetTrackAudioFile;

public class GetTrackAudioFileQueryHandler : IQueryHandler<GetTrackAudioFileQuery, GetTrackAudioFileResult>
{
	private static readonly TimeSpan StreamUrlExpiration = TimeSpan.FromMinutes(10);

	private readonly IUnitOfWork _unitOfWork;
	private readonly IStorageService _storageService;

	public GetTrackAudioFileQueryHandler(IUnitOfWork unitOfWork,
										 IStorageService storageService)
	{
		_unitOfWork = unitOfWork;
		_storageService = storageService;
	}


	public async Task<Result<GetTrackAudioFileResult>> Handle(GetTrackAudioFileQuery query,
															  CancellationToken ct = default)
	{
		Track? track = await _unitOfWork.Repository<ITrackReadRepository>()
										.GetTrackDetailAsync(id: query.TrackId,
															 cancellationToken: ct);
		if (track is null)
			return Result<GetTrackAudioFileResult>.Failure(
				TrackErrors.NotFound());


		StorageFileReference fileReference = new StorageFileReference
		(
			StorageProvider: track.AudioFile.StorageProvider,
			ContainerName: track.AudioFile.ContainerName,
			StoredFileName: track.AudioFile.StoredFileName
		);


		Result<StorageStreamResult> streamResult = await _storageService.OpenReadStreamAsync(fileReference: fileReference,
																									rangeHeader: query.RangeHeader,
																									urlExpiration: StreamUrlExpiration,
																									cancellationToken: ct);

		if (streamResult.IsFailure)
			return Result<GetTrackAudioFileResult>.Failure(streamResult.Error);

		string contentType = streamResult.Value.ContentType;


		if (string.IsNullOrWhiteSpace(contentType) || contentType == "application/octet-stream")
			contentType = track.AudioFile.ContentType ?? "audio/mpeg";

		if (string.IsNullOrWhiteSpace(contentType) || contentType == "application/octet-stream")
			contentType = track.AudioFile.UploadedFileName.EndsWith(value: "wav",
																	comparisonType: StringComparison.OrdinalIgnoreCase)
				? "audio/wav"
				: "audio/mpeg";


		return Result<GetTrackAudioFileResult>.Success(
			new GetTrackAudioFileResult(
				FileName: track.AudioFile.UploadedFileName,
				ContentType: contentType,
				StorageStreamResult: streamResult.Value
			)
		);
		
	}

}
