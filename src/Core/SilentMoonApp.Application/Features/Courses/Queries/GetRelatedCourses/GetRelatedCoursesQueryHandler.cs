using SilentMoonApp.Application.Abstractions.Executors;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Domain.Entities;

namespace SilentMoonApp.Application.Features.Courses.Queries.GetRelatedCourses;

public class GetRelatedCoursesQueryHandler : IQueryHandler<GetRelatedCoursesQuery, IReadOnlyList<GetRelatedCoursesResult>>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IStorageService _storageService;
	private readonly IQueryExecutor _queryExecutor;

	public GetRelatedCoursesQueryHandler(IUnitOfWork unitOfWork,
										 IStorageService storageService,
										 IQueryExecutor queryExecutor)
	{
		_unitOfWork = unitOfWork;
		_storageService = storageService;
		_queryExecutor = queryExecutor;
	}


	public async Task<Result<IReadOnlyList<GetRelatedCoursesResult>>> Handle(GetRelatedCoursesQuery query,
																			 CancellationToken ct = default)
	{
		int limit = query.Limit <= 0
				  ? 20
				  : Math.Min(query.Limit, 50);

		Course? currentCourse = await _unitOfWork.Repository<ICourseReadRepository>()
										  .GetCourseDetailAsync(id: query.Id,
																tracking: false,
																cancellationToken: ct);

		if (currentCourse is null)
			return Result<IReadOnlyList<GetRelatedCoursesResult>>.Failure(
				CourseErrors.NotFound());

		IQueryable<Course> relatedCoursesQuery = _unitOfWork.Repository<ICourseReadRepository>()
															.QueryForList(filter: course => !course.IsDeleted
																						 && !course.Category.IsDeleted
																						 && !course.Category.CategoryType.IsDeleted
																						 && course.Id != currentCourse.Id
																						 && course.Category.CategoryTypeId == currentCourse.Category.CategoryTypeId,
																		  tracking: false)
															.OrderByDescending(course => course.CategoryId == currentCourse.CategoryId)
															.ThenByDescending(course => course.CreatedAt)
															.Take(limit);

		List<Course> relatedCourses = await _queryExecutor.ToListAsync(query: relatedCoursesQuery,
																	   cancellationToken: ct);

		GetRelatedCoursesResult[] data = await Task.WhenAll(
			relatedCourses.Select(async course =>
			{
				string imageUrl = string.Empty;

				if (!course.CoverImageFile.IsDeleted)
				{
					Result<string> imageUrlResult = await _storageService.GetFileUrlAsync(
						fileReference: new StorageFileReference(StorageProvider: course.CoverImageFile.StorageProvider,
																ContainerName: course.CoverImageFile.ContainerName,
																StoredFileName: course.CoverImageFile.StoredFileName),
						urlExpiration: TimeSpan.FromMinutes(10),
						cancellationToken: ct);


					imageUrl = imageUrlResult.IsSuccess
							 ? imageUrlResult.Value
							 : string.Empty;
				}


				return new GetRelatedCoursesResult
				(
					Id: course.Id,
					Title: course.Title,
					SubTitle: course.SubTitle,
					Type: course.Category.CategoryType.Slug,
					CategoryId: course.CategoryId,
					ImageUrl: imageUrl,
					DurationSec: course.DurationSec,
					IsFeatured: course.IsFeatured,
					Narrators: course.Tracks.Where(track => !track.IsDeleted
														 && !track.Narrator.IsDeleted)
											.Select(track => track.Narrator.Slug)
											.Distinct()
											.ToList()
				);
			})
		);


		return Result<IReadOnlyList<GetRelatedCoursesResult>>.Success(data);
	}
}
