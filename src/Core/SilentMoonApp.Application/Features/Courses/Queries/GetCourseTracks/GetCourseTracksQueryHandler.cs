using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Domain.Entities;


namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourseTracks;

public class GetCourseTracksQueryHandler : IQueryHandler<GetCourseTracksQuery, GetCourseTracksResult>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IStorageService _storageService;

	public GetCourseTracksQueryHandler(IUnitOfWork unitOfWork,
									   IStorageService storageService)
	{
		_unitOfWork = unitOfWork;
		_storageService = storageService;
	}


	public async Task<Result<GetCourseTracksResult>> Handle(GetCourseTracksQuery query,
															CancellationToken ct = default)
	{
		Course? course = await _unitOfWork.Repository<ICourseReadRepository>()
										  .GetCourseDetailAsync(id: query.Id,
																tracking: false,
																cancellationToken: ct);
		if (course is null)
			return Result<GetCourseTracksResult>.Failure(
				CourseErrors.NotFound());


		string courseImageUrl = string.Empty;

		if (!course.CoverImageFile.IsDeleted)
		{
			Result<string> courseImageUrlResult = await _storageService.GetFileUrlAsync(
				fileReference: new StorageFileReference(StorageProvider: course.CoverImageFile.StorageProvider,
														ContainerName: course.CoverImageFile.ContainerName,
														StoredFileName: course.CoverImageFile.StoredFileName),
				urlExpiration: TimeSpan.FromMinutes(10),
				cancellationToken: ct);


			courseImageUrl = courseImageUrlResult.IsSuccess
					 ? courseImageUrlResult.Value
					 : string.Empty;
		}

		IReadOnlyList<Track> tracks = course.Tracks.Where(track => !track.IsDeleted
																&& !track.Narrator.IsDeleted
																&& track.AudioFile is not null
																&& !track.AudioFile.IsDeleted
																&& (query.NarratorId is null || track.NarratorId == query.NarratorId))
												   .OrderBy(track => track.Order)
												   .ToList();


		GetCourseTrackItemResult[] data = await Task.WhenAll(
			tracks.Select(async track =>
			{
				string audioUrl = string.Empty;
				string trackImageUrl = string.Empty;

				Result<string> audioUrlResult = await _storageService.GetFileUrlAsync(
					fileReference: new StorageFileReference(StorageProvider: track.AudioFile.StorageProvider,
															ContainerName: track.AudioFile.ContainerName,
															StoredFileName: track.AudioFile.StoredFileName),
					urlExpiration: TimeSpan.FromMinutes(10),
					cancellationToken: ct);

				audioUrl = audioUrlResult.IsSuccess
						 ? audioUrlResult.Value
						 : string.Empty;


				if (track.CoverImageFile is not null && !track.CoverImageFile.IsDeleted)
				{
					Result<string> imageUrlResult = await _storageService.GetFileUrlAsync(
						fileReference: new StorageFileReference(StorageProvider: track.CoverImageFile.StorageProvider,
																ContainerName: track.CoverImageFile.ContainerName,
																StoredFileName: track.CoverImageFile.StoredFileName),
						urlExpiration: TimeSpan.FromMinutes(10),
						cancellationToken: ct);

					trackImageUrl = imageUrlResult.IsSuccess
							 ? imageUrlResult.Value
							 : courseImageUrl;
				}


				return new GetCourseTrackItemResult
				(
					Id: track.Id,
					CourseId: course.Id,
					Title: track.Title,
					NarratorId: track.NarratorId,
					NarratorName: track.Narrator.Name,
					NarratorSlug: track.Narrator.Slug,
					DurationSec: track.AudioFile?.DurationSec ?? 0,
					AudioUrl: audioUrl,
					MimeType: track.AudioFile?.ContentType ?? string.Empty,
					FileSizeBytes: track.AudioFile?.SizeBytes ?? 0,
					ImageUrl: trackImageUrl,
					TrackNumber: track.Order
				);
			}
		));


		return Result<GetCourseTracksResult>.Success(
			new GetCourseTracksResult(Tracks: data));
	}
}
