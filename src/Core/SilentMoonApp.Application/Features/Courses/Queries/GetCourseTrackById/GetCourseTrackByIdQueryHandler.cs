using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Domain.Entities;

namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourseTrackById;

public sealed class GetCourseTrackByIdQueryHandler : IQueryHandler<GetCourseTrackByIdQuery, GetCourseTrackByIdResult>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IStorageService _storageService;

	public GetCourseTrackByIdQueryHandler(IUnitOfWork unitOfWork,
										  IStorageService storageService)
	{
		_unitOfWork = unitOfWork;
		_storageService = storageService;
	}


	public async Task<Result<GetCourseTrackByIdResult>> Handle(GetCourseTrackByIdQuery query,
														 CancellationToken ct = default)
	{
		Track? track = await _unitOfWork.Repository<ITrackReadRepository>()
								  .GetTrackDetailAsync(id: query.Id,
													   tracking: false,
													   cancellationToken: ct);


		if (track is null)
			return Result<GetCourseTrackByIdResult>.Failure(
				TrackErrors.NotFound());


		string audioUrl = string.Empty;

		Result<string> audioUrlResult = await _storageService.GetFileUrlAsync(
			fileReference: new StorageFileReference(StorageProvider: track.AudioFile.StorageProvider,
													ContainerName: track.AudioFile.ContainerName,
													StoredFileName: track.AudioFile.StoredFileName),
			urlExpiration: TimeSpan.FromMinutes(10),
			cancellationToken: ct);

		audioUrl = audioUrlResult.IsSuccess
				 ? audioUrlResult.Value
				 : string.Empty;

		string imageUrl = string.Empty;

		if (track.CoverImageFile is not null &&
		   !track.CoverImageFile.IsDeleted)
		{
			Result<string> trackImageUrlResult = await _storageService.GetFileUrlAsync(
				fileReference: new StorageFileReference(StorageProvider: track.CoverImageFile.StorageProvider,
														ContainerName: track.CoverImageFile.ContainerName,
														StoredFileName: track.CoverImageFile.StoredFileName),
				urlExpiration: TimeSpan.FromMinutes(10),
				cancellationToken: ct);

			imageUrl = trackImageUrlResult.IsSuccess
					 ? trackImageUrlResult.Value
					 : string.Empty;
		}

		else if (!track.Course.CoverImageFile.IsDeleted)
		{
			Result<string> courseImageUrlResult = await _storageService.GetFileUrlAsync(
							fileReference: new StorageFileReference(StorageProvider: track.Course.CoverImageFile.StorageProvider,
																	ContainerName: track.Course.CoverImageFile.ContainerName,
																	StoredFileName: track.Course.CoverImageFile.StoredFileName),
							urlExpiration: TimeSpan.FromMinutes(10),
							cancellationToken: ct);

			imageUrl = courseImageUrlResult.IsSuccess
					 ? courseImageUrlResult.Value
					 : string.Empty;
		}


		return Result<GetCourseTrackByIdResult>.Success(
			new GetCourseTrackByIdResult
			(
				Id: track.Id,
				CourseId: track.CourseId,
				Title: track.Title,
				NarratorId: track.NarratorId,
				NarratorName: track.Narrator.Name,
				NarratorSlug: track.Narrator.Slug,
				DurationSec: track.AudioFile.DurationSec,
				AudioUrl: audioUrl,
				MimeType: track.AudioFile.ContentType,
				FileSizeBytes: track.AudioFile.SizeBytes,
				ImageUrl: imageUrl,
				TrackNumber: track.Order
			)
		);
	}
}
