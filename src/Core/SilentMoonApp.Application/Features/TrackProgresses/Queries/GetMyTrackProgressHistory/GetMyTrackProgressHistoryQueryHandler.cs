using SilentMoonApp.Domain.Entities;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.Extensions;
using SilentMoonApp.Application.DTOs.Common;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.Abstractions.Executors;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Features.TrackProgresses.Queries.GetMyTrackProgress;


namespace SilentMoonApp.Application.Features.TrackProgresses.Queries.GetMyTrackProgressHistory;

public class GetMyTrackProgressHistoryQueryHandler : IQueryHandler<GetMyTrackProgressHistoryQuery, GetMyTrackProgressHistoryResult>
{
	private static readonly TimeSpan UrlExpiration = TimeSpan.FromMinutes(10);

	private readonly IUnitOfWork _unitOfWork;
	private readonly ICurrentUser _currentUser;
	private readonly IQueryExecutor _queryExecutor;
	private readonly IStorageService _storageService;

	public GetMyTrackProgressHistoryQueryHandler(IUnitOfWork unitOfWork,
												 ICurrentUser currentUser,
												 IQueryExecutor queryExecutor,
												 IStorageService storageService)
	{
		_unitOfWork = unitOfWork;
		_currentUser = currentUser;
		_queryExecutor = queryExecutor;
		_storageService = storageService;
	}


	public async Task<Result<GetMyTrackProgressHistoryResult>> Handle(GetMyTrackProgressHistoryQuery query,
																CancellationToken ct = default)
	{
		if (!_currentUser.IsAuthenticated || _currentUser.UserId is not Guid userId)
			return Result<GetMyTrackProgressHistoryResult>.Failure(AuthErrors.UnAuthorized());


		User? user = await _unitOfWork.ReadRepository<User>()
										.GetByIdAsync(id: userId,
													  tracking: false,
													  cancellationToken: ct);
		if (user is null || user.IsDeleted)
			return Result<GetMyTrackProgressHistoryResult>.Failure(AuthErrors.UnAuthorized());


		IQueryable<TrackProgress> historyQuery = _unitOfWork.Repository<ITrackProgressReadRepository>()
															.QueryMyHistory(userId: userId)
															.OrderByDescending(tp => tp.UpdatedAt);

		PaginationResult<TrackProgress> paginatedHistory = await historyQuery.PaginateAsync(queryExecutor: _queryExecutor,
																						   paginationRequest: query.PaginationQueryRequest,
																						   cancellationToken: ct);

		GetMyTrackProgressHistoryItemResult[] data = await Task.WhenAll(
			paginatedHistory.Data.Select(async progress =>
			{
				Track track = progress.Track;

				string audioUrl = string.Empty;
				string imageUrl = string.Empty;

				Result<string> audioUrlResult = await _storageService.GetFileUrlAsync(fileReference: new StorageFileReference(StorageProvider: track.AudioFile.StorageProvider,
																															  ContainerName: track.AudioFile.ContainerName,
																															  StoredFileName: track.AudioFile.StoredFileName),
																					  urlExpiration: UrlExpiration,
																					  cancellationToken: ct);

				if (audioUrlResult.IsSuccess)
					audioUrl = audioUrlResult.Value;

				if (track.CoverImageFile is not null && !track.CoverImageFile.IsDeleted)
				{
					Result<string> imageUrlResult = await _storageService.GetFileUrlAsync(fileReference: new StorageFileReference(StorageProvider: track.CoverImageFile.StorageProvider,
																																  ContainerName: track.CoverImageFile.ContainerName,
																																  StoredFileName: track.CoverImageFile.StoredFileName),
																						  urlExpiration: UrlExpiration,
																						  cancellationToken: ct);
					if (imageUrlResult.IsSuccess)
						imageUrl = imageUrlResult.Value;
				}

				else if (!track.Course.CoverImageFile.IsDeleted)
				{
					Result<string> imageUrlResult = await _storageService.GetFileUrlAsync(fileReference: new StorageFileReference(StorageProvider: track.Course.CoverImageFile.StorageProvider,
																																  ContainerName: track.Course.CoverImageFile.ContainerName,
																																  StoredFileName: track.Course.CoverImageFile.StoredFileName),
																						  urlExpiration: UrlExpiration,
																						  cancellationToken: ct);
					if (imageUrlResult.IsSuccess)
						imageUrl = imageUrlResult.Value;
				}


				return new GetMyTrackProgressHistoryItemResult
				(
					Progress: new GetMyTrackProgressResult
					(
						Id: progress.Id,
						TrackId: progress.TrackId,
						PositionSec: progress.PositionSec,
						Completed: progress.Completed,
						UpdatedAt: progress.UpdatedAt
					),

					Track: new GetMyTrackProgressHistoryTrackResult
					(
						Id: track.Id,
						CourseId: track.CourseId,
						Title: track.Title,
						Narrator: track.Narrator.Slug,
						DurationSec: track.AudioFile.DurationSec,
						AudioUrl: audioUrl,
						MimeType: track.AudioFile.ContentType,
						FileSizeBytes: track.AudioFile.SizeBytes,
						ImageUrl: imageUrl,
						TrackNumber: track.Order
					)
				);
			})
		);


		return Result<GetMyTrackProgressHistoryResult>.Success
		(
			new GetMyTrackProgressHistoryResult
			(
				PaginationResult: new PaginationResult<GetMyTrackProgressHistoryItemResult>
				{
					Data = data,
					Meta = paginatedHistory.Meta
				}
			)
		);
	}

}
