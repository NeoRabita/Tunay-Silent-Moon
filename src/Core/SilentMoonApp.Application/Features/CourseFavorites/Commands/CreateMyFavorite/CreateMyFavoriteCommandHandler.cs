using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Domain.Entities;

namespace SilentMoonApp.Application.Features.CourseFavorites.Commands.CreateMyFavorite;

public class CreateMyFavoriteCommandHandler : ICommandHandler<CreateMyFavoriteCommand, CreateMyFavoriteResult>
{
	private static readonly TimeSpan UrlExpiration = TimeSpan.FromMinutes(10);

	private readonly IUnitOfWork _unitOfWork;
	private readonly ICurrentUser _currentUser;
	private readonly TimeProvider _timeProvider;
	private readonly IStorageService _storageService;

	public CreateMyFavoriteCommandHandler(IUnitOfWork unitOfWork,
										  ICurrentUser currentUser,
										  TimeProvider timeProvider,
										  IStorageService storageService)
	{
		_unitOfWork = unitOfWork;
		_currentUser = currentUser;
		_timeProvider = timeProvider;
		_storageService = storageService;
	}


	public async Task<Result<CreateMyFavoriteResult>> Handle(CreateMyFavoriteCommand command,
													   CancellationToken ct = default)
	{
		if (!_currentUser.IsAuthenticated || _currentUser.UserId is not Guid userId)
			return Result<CreateMyFavoriteResult>.Failure(AuthErrors.UnAuthorized());

		User? user = await _unitOfWork.ReadRepository<User>()
			.GetByIdAsync(userId, tracking: false, cancellationToken: ct);

		if (user is null || user.IsDeleted)
			return Result<CreateMyFavoriteResult>.Failure(AuthErrors.UnAuthorized());


		Course? course = await _unitOfWork.Repository<ICourseReadRepository>()
										 .GetCourseDetailAsync(id: command.CourseId,
															   tracking: false,
															   cancellationToken: ct);

		if (course is null)
			return Result<CreateMyFavoriteResult>.Failure(
				CourseErrors.NotFound());

		bool alreadyExists = await _unitOfWork.ReadRepository<CourseFavorite>()
											  .AnyAsync(filter: favorite => favorite.UserId == userId
																		&& favorite.CourseId == command.CourseId,
														cancellationToken: ct);

		if (alreadyExists)
			return Result<CreateMyFavoriteResult>.Failure(
				CourseFavoriteErrors.AlreadyExists());

		DateTimeOffset now = _timeProvider.GetUtcNow();

		CourseFavorite courseFavorite = new()
		{
			UserId = userId,
			CourseId = command.CourseId,
			CreatedAt = now
		};

		await _unitOfWork.WriteRepository<CourseFavorite>()
						.AddAsync(entity: courseFavorite,
								  cancellationToken: ct);

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


		return Result<CreateMyFavoriteResult>.Success(
			new CreateMyFavoriteResult(
				Id: courseFavorite.Id,
				CourseId: courseFavorite.CourseId,
				Course: new CreateMyFavoriteCourseResult(
					Id: course.Id,
					Title: course.Title,
					SubTitle: course.SubTitle,
					CategoryTypeId: course.Category.CategoryTypeId,
					CategoryId: course.CategoryId,
					ImageUrl: imageUrl,
					DurationSec: course.DurationSec,
					IsFeatured: course.IsFeatured,
					Narrators: course.Tracks
						.Where(track => !track.IsDeleted && !track.Narrator.IsDeleted)
						.Select(track => track.Narrator.Slug)
						.Distinct()
						.ToList()),
				CreatedAt: courseFavorite.CreatedAt
			)
		);
	}

}
