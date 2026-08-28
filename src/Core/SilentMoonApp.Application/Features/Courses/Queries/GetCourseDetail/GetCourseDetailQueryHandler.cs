using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.Extensions;
using SilentMoonApp.Domain.Entities;


namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourseDetail;

public class GetCourseDetailQueryHandler : IQueryHandler<GetCourseDetailQuery, GetCourseDetailResult>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IStorageService _storageService;
	private readonly ICurrentUser _currentUser;

	public GetCourseDetailQueryHandler(IUnitOfWork unitOfWork,
									   IStorageService storageService,
									   ICurrentUser currentUser)
	{
		_unitOfWork = unitOfWork;
		_storageService = storageService;
		_currentUser = currentUser;
	}


	public async Task<Result<GetCourseDetailResult>> Handle(GetCourseDetailQuery query, CancellationToken ct = default)
	{
		Guid userId = _currentUser.GetRequiredUserId();

		Course? course = await _unitOfWork.Repository<ICourseReadRepository>()
										  .GetCourseDetailAsync(id: query.Id,
																tracking: false,
																cancellationToken: ct);
		if (course is null)
			return Result<GetCourseDetailResult>.Failure(
				CourseErrors.NotFound());


		string courseImageUrl = string.Empty;

		if (!course.CoverImageFile.IsDeleted)
		{
			Result<string> imageUrlResult = await _storageService.GetFileUrlAsync(fileReference: new StorageFileReference(StorageProvider: course.CoverImageFile.StorageProvider,
																														  ContainerName: course.CoverImageFile.ContainerName,
																														  StoredFileName: course.CoverImageFile.StoredFileName),
																				  urlExpiration: TimeSpan.FromMinutes(10),
																				  cancellationToken: ct);

			courseImageUrl = imageUrlResult.IsSuccess
					 ? imageUrlResult.Value
					 : string.Empty;
		}


		IReadOnlyList<Track> availableTracks = course.Tracks.Where(track => !track.IsDeleted
											   && !track.Narrator.IsDeleted
											   && track.AudioFile is not null
											   && !track.AudioFile.IsDeleted)
								  .OrderBy(track => track.Order)
								  .ToList();


		IReadOnlyList<Track> filteredTracks = availableTracks.Where(track => query.NarratorId is null
																		  || track.NarratorId == query.NarratorId)
															 .ToList();


		IReadOnlyList<GetCourseDetailNarratorResult> narrators = availableTracks.Select(track => new GetCourseDetailNarratorResult(Id: track.Narrator.Id,
																																   Name: track.Narrator.Name,
																																   Slug: track.Narrator.Slug))
																				.DistinctBy(narrator => narrator.Id)
																				.ToList();


		GetCourseDetailTrackResult[] tracks = await Task.WhenAll(
			filteredTracks.Select(async track =>
			{
				string audioUrl = string.Empty;
				string imageUrl = courseImageUrl;

				Result<string> audioUrlResult = await _storageService.GetFileUrlAsync(fileReference: new StorageFileReference(StorageProvider: track.AudioFile.StorageProvider,
																														   ContainerName: track.AudioFile.ContainerName,
																														   StoredFileName: track.AudioFile.StoredFileName),
																					  urlExpiration: TimeSpan.FromMinutes(10),
																					  cancellationToken: ct);

				audioUrl = audioUrlResult.IsSuccess
						 ? audioUrlResult.Value
						 : string.Empty;


				if (track.CoverImageFile is not null && !track.CoverImageFile.IsDeleted)
				{
					Result<string> imageUrlResult = await _storageService.GetFileUrlAsync(fileReference: new StorageFileReference(StorageProvider: track.CoverImageFile.StorageProvider,
																																	ContainerName: track.CoverImageFile.ContainerName,
																																	StoredFileName: track.CoverImageFile.StoredFileName),
																							urlExpiration: TimeSpan.FromMinutes(10),
																							cancellationToken: ct);

					imageUrl = imageUrlResult.IsSuccess
							 ? imageUrlResult.Value
							 : courseImageUrl;
				}


				return new GetCourseDetailTrackResult
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
				);

			})
		);


		List<Guid> trackIds = filteredTracks.Select(track => track.Id).ToList();

		TrackProgress? userProgress = trackIds.Count == 0
									? null
									: (await _unitOfWork.ReadRepository<TrackProgress>()
														.GetAllAsync(filter: progress => progress.UserId == userId
														 							 && trackIds.Contains(progress.TrackId),
														 			tracking: false,
														 			cancellationToken: ct))
														.OrderByDescending(progress => progress.UpdatedAt)
														.FirstOrDefault();

		bool isFavorited = await _unitOfWork.ReadRepository<CourseFavorite>()
										   .AnyAsync(filter: favorite => favorite.UserId == userId
																	  && favorite.CourseId == course.Id,
													  cancellationToken: ct);

		return Result<GetCourseDetailResult>.Success(
		new GetCourseDetailResult
		(
			Course: new GetCourseDetailCourseResult
			(
				Id: course.Id,
				Title: course.Title,
				SubTitle: course.SubTitle,
				CategoryType: course.Category.CategoryType.Slug,
				CategoryId: course.CategoryId,
				ImageUrl: courseImageUrl,
				DurationSec: course.DurationSec,
				IsFeatured: course.IsFeatured,
				Narrators: narrators,
				Description: course.Description,
				TrackCount: availableTracks.Count,
				CreatedAt: course.CreatedAt
			),
			Tracks: tracks,
			UserProgress: userProgress is null
				? null
				: new GetCourseDetailUserProgressResult
				(
					Id: userProgress.Id,
					TrackId: userProgress.TrackId,
					PositionSec: userProgress.PositionSec,
					Completed: userProgress.Completed,
					UpdatedAt: userProgress.UpdatedAt
				),
			IsFavorited: isFavorited
		)
	);
	}

}
