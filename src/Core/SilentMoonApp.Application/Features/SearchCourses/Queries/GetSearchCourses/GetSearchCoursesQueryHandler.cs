using SilentMoonApp.Domain.Entities;
using SilentMoonApp.Application.Extensions;
using SilentMoonApp.Application.DTOs.Common;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.Abstractions.Executors;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;


namespace SilentMoonApp.Application.Features.SearchCourses.Queries.GetSearchCourses;

public class GetSearchCoursesQueryHandler : IQueryHandler<GetSearchCoursesQuery, GetSearchCoursesResult>
{
	private static readonly TimeSpan UrlExpiration = TimeSpan.FromMinutes(10);

	private readonly IUnitOfWork _unitOfWork;
	private readonly IQueryExecutor _queryExecutor;
	private readonly IStorageService _storageService;

	public GetSearchCoursesQueryHandler(IUnitOfWork unitOfWork,
										IQueryExecutor queryExecutor,
										IStorageService storageService)
	{
		_unitOfWork = unitOfWork;
		_queryExecutor = queryExecutor;
		_storageService = storageService;
	}


	public async Task<Result<GetSearchCoursesResult>> Handle(GetSearchCoursesQuery query,
													   CancellationToken ct = default)
	{
		string search = query.Search?.Trim().ToLowerInvariant()
					 ?? string.Empty;


		IQueryable<Course> coursesQuery = _unitOfWork.Repository<ICourseReadRepository>()
													 .QueryForList(filter: course => !course.IsDeleted
																				  && !course.Category.IsDeleted
																				  && !course.Category.CategoryType.IsDeleted
																				  && (query.CategoryTypeId == null || course.Category.CategoryType.Id == query.CategoryTypeId)
																				  && (string.IsNullOrEmpty(search) || course.Title.ToLower().Contains(search)
																												   || course.SubTitle.ToLower().Contains(search)
																												   || course.Description.ToLower().Contains(search)),
																   tracking: false)
													 .OrderByDescending(course => course.IsFeatured)
														.ThenByDescending(course => course.CreatedAt);


		PaginationResult<Course> paginatedCourses = await coursesQuery.PaginateAsync(queryExecutor: _queryExecutor,
																					 paginationRequest: query.PaginationQueryRequest,
																					 cancellationToken: ct);

		GetSearchCourseItemResult[] courses = await Task.WhenAll
		(
			paginatedCourses.Data.Select(async course =>
			{
				string imageUrl = string.Empty;

				if (course.CoverImageFile is not null)
				{
					Result<string> imageUrlResult = await _storageService.GetFileUrlAsync(fileReference: new StorageFileReference(StorageProvider: course.CoverImageFile.StorageProvider,
																																  ContainerName: course.CoverImageFile.ContainerName,
																																  StoredFileName: course.CoverImageFile.StoredFileName),
																						  urlExpiration: UrlExpiration,
																						  cancellationToken: ct);

					imageUrl = imageUrlResult.IsSuccess
						? imageUrlResult.Value
						: string.Empty;
				}


				return new GetSearchCourseItemResult
				(
					Id: course.Id,
					Title: course.Title,
					SubTitle: course.SubTitle,
					Type: course.Category.CategoryType.Slug,
					CategoryId: course.CategoryId,
					ImageUrl: imageUrl,
					DurationSec: course.DurationSec,
					IsFeatured: course.IsFeatured,
					Narrators: course.Tracks.Where(track => !track.IsDeleted && !track.Narrator.IsDeleted)
											.Select(track => track.Narrator.Slug)
											.Distinct()
											.ToList()
				);
			})
		);


		return Result<GetSearchCoursesResult>.Success
		(
			new GetSearchCoursesResult
			(
				Search: query.Search!.Trim(),
				PaginationResult: new PaginationResult<GetSearchCourseItemResult>
				{
					Data = courses,
					Meta = paginatedCourses.Meta
				}
			)
		);
	}
}