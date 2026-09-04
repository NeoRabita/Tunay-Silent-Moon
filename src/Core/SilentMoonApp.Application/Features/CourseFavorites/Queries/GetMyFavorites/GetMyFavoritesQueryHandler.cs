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


namespace SilentMoonApp.Application.Features.CourseFavorites.Queries.GetMyFavorites;

public class GetMyFavoritesQueryHandler : IQueryHandler<GetMyFavoritesQuery, GetMyFavoritesResult>
{
	private static readonly TimeSpan UrlExpiration = TimeSpan.FromMinutes(10);

	private readonly IUnitOfWork _unitOfWork;
	private readonly ICurrentUser _currentUser;
	private readonly IQueryExecutor _queryExecutor;
	private readonly IStorageService _storageService;

	public GetMyFavoritesQueryHandler(IUnitOfWork unitOfWork,
									  ICurrentUser currentUser,
									  IQueryExecutor queryExecutor,
									  IStorageService storageService)
	{
		_unitOfWork = unitOfWork;
		_currentUser = currentUser;
		_queryExecutor = queryExecutor;
		_storageService = storageService;
	}


	public async Task<Result<GetMyFavoritesResult>> Handle(GetMyFavoritesQuery query,
													 CancellationToken ct = default)
	{
		if (!_currentUser.IsAuthenticated || _currentUser.UserId is not Guid userId)

			return Result<GetMyFavoritesResult>.Failure(
				AuthErrors.UnAuthorized());

		User? user = await _unitOfWork.ReadRepository<User>()
									  .GetByIdAsync(id: userId,
													tracking: false,
													cancellationToken: ct);
		if (user is null || user.IsDeleted)
			return Result<GetMyFavoritesResult>.Failure(
				AuthErrors.UnAuthorized());

		IQueryable<CourseFavorite> favoritesQuery = _unitOfWork.Repository<ICourseFavoriteReadRepository>()
																   .QueryMyFavorites(userId: userId,
																					 categoryTypeId: query.CategoryTypeId)
																   .OrderByDescending(favorite => favorite.CreatedAt);

		PaginationResult<CourseFavorite> paginatedFavorites = await favoritesQuery.PaginateAsync(queryExecutor: _queryExecutor,
																						   paginationRequest: query.PaginationQueryRequest,
																						   cancellationToken: ct);

		GetMyFavoriteItemResult[] favorites = await Task.WhenAll(
			paginatedFavorites.Data.Select(async favorite =>
			{
				Course course = favorite.Course;

				string imageUrl = string.Empty;

				if (course.CoverImageFile is not null && !course.CoverImageFile.IsDeleted)
				{
					Result<string> imageUrlResult = await _storageService.GetFileUrlAsync(
						fileReference: new StorageFileReference(
							StorageProvider: course.CoverImageFile.StorageProvider,
							ContainerName: course.CoverImageFile.ContainerName,
							StoredFileName: course.CoverImageFile.StoredFileName),
						urlExpiration: UrlExpiration,
						cancellationToken: ct);

					if (imageUrlResult.IsSuccess)
						imageUrl = imageUrlResult.Value;
				}

				return new GetMyFavoriteItemResult
				(
					Id: favorite.Id,
					CourseId: course.Id,
					Course: new GetMyFavoriteCourseResult
					(
						Id: course.Id,
						Title: course.Title,
						SubTitle: course.SubTitle,
						CategoryTypeId: course.Category.CategoryTypeId,
						CategoryId: course.CategoryId,
						ImageUrl: imageUrl,
						DurationSec: course.DurationSec,
						IsFeatured: course.IsFeatured,
						Narrators: course.Tracks.Where(track => !track.IsDeleted && !track.Narrator.IsDeleted)
												.Select(track => track.Narrator.Slug)
												.Distinct()
												.ToList()
					),

					CreatedAt: favorite.CreatedAt
				);
			})
		);


		return Result<GetMyFavoritesResult>.Success(
			new GetMyFavoritesResult(
				PaginationResult: new PaginationResult<GetMyFavoriteItemResult>
				{
					Data = favorites,
					Meta = paginatedFavorites.Meta
				}
			)
		);
	}

}
