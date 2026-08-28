using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Domain.Entities;

namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourseWithNarrators;


public class GetCourseWithNarratorsQueryHandler : IQueryHandler<GetCourseWithNarratorsQuery, GetCourseWithNarratorsResult>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IStorageService _storageService;

	public GetCourseWithNarratorsQueryHandler(IUnitOfWork unitOfWork,
											  IStorageService storageService)
	{
		_unitOfWork = unitOfWork;
		_storageService = storageService;
	}


	public async Task<Result<GetCourseWithNarratorsResult>> Handle(GetCourseWithNarratorsQuery query, CancellationToken ct = default)
	{
		Course? course = await _unitOfWork.Repository<ICourseReadRepository>()
										  .GetCourseDetailAsync(id: query.Id,
																tracking: false,
																cancellationToken: ct);
		if (course is null)
			return Result<GetCourseWithNarratorsResult>.Failure(
				CourseErrors.NotFound());


		string imageUrl = string.Empty;

		if (!course.CoverImageFile.IsDeleted)
		{
			Result<string> imageUrlResult = await _storageService.GetFileUrlAsync(fileReference: new StorageFileReference(StorageProvider: course.CoverImageFile.StorageProvider,
																														  ContainerName: course.CoverImageFile.ContainerName,
																														  StoredFileName: course.CoverImageFile.StoredFileName),
																				  urlExpiration: TimeSpan.FromMinutes(10),
																				  cancellationToken: ct);

			imageUrl = imageUrlResult.IsSuccess
					 ? imageUrlResult.Value
					 : string.Empty;
		}


		IReadOnlyList<Track> tracks = course.Tracks?.Where(track => !track.IsDeleted
																 && !track.Narrator.IsDeleted)
													.ToList()
								   ?? new List<Track>();


		return Result<GetCourseWithNarratorsResult>.Success(
		new GetCourseWithNarratorsResult
		(
			Id: course.Id,
			Title: course.Title,
			SubTitle: course.SubTitle,
			CategoryType: course.Category.CategoryType.Slug,
			CategoryId: course.CategoryId,
			ImageUrl: imageUrl,
			DurationSec: course.DurationSec,
			IsFeatured: course.IsFeatured,
			Narrators: tracks.Select(track => track.Narrator.Slug)
							 .Distinct()
							 .ToList(),
			Description: course.Description,
			TrackCount: tracks.Count,
			CreatedAt: course.CreatedAt
		));
	}

}
