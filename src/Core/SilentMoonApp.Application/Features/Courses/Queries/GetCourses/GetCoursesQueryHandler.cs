using SilentMoonApp.Application.Abstractions.Executors;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.DTOs.Common;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Extensions;
using SilentMoonApp.Domain.Entities;
using System.Linq.Expressions;


namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourses;

public class GetCoursesQueryHandler : IQueryHandler<GetCoursesQuery, GetCoursesResult>
{
	private static readonly IReadOnlyDictionary<ECourseSortBy, Expression<Func<Course, object>>> SortByExpressions
		= new Dictionary<ECourseSortBy, Expression<Func<Course, object>>>
		{
			[ECourseSortBy.CreatedAt] = course => course.CreatedAt,
			[ECourseSortBy.Title] = course => course.Title,
			[ECourseSortBy.Popularity] = course => course.IsFeatured
		};

	private readonly IUnitOfWork _unitOfWork;
	private readonly IStorageService _storageService;
	private readonly IQueryExecutor _queryExecutor;

	public GetCoursesQueryHandler(IUnitOfWork unitOfWork,
								  IStorageService storageService,
								  IQueryExecutor queryExecutor)
	{
		_unitOfWork = unitOfWork;
		_storageService = storageService;
		_queryExecutor = queryExecutor;
	}


	public async Task<Result<GetCoursesResult>> Handle(GetCoursesQuery query, CancellationToken ct = default)
	{
		string? categoryType = string.IsNullOrWhiteSpace(query.CategoryType)
							 ? null
							 : query.CategoryType.Trim().ToLowerInvariant();

		string? search = string.IsNullOrWhiteSpace(query.Search)
						? null
						: query.Search.Trim().ToLowerInvariant();


		IQueryable<Course> coursesQuery = _unitOfWork.Repository<ICourseReadRepository>()
													 .QueryForList(filter: course => !course.IsDeleted
																				  && !course.Category.IsDeleted
																				  && !course.Category.CategoryType.IsDeleted
																				  && (categoryType == null || course.Category.CategoryType.Slug == categoryType)
																				  && (query.CategoryId == null || course.CategoryId == query.CategoryId)
																				  && (query.IsFeatured == null || course.IsFeatured == query.IsFeatured)
																				  && (search == null || course.Title.ToLower().Contains(search) ||
																										course.SubTitle.ToLower().Contains(search)
																										/*course.Description.ToLower().Contains(search)*/),
																   tracking: false);


		coursesQuery = coursesQuery.ApplySort(sortBy: query.CourseSortBy,
											  defaultSortBy: ECourseSortBy.CreatedAt,
											  sortFilter: SortByExpressions,
											  sortDirection: query.SortDirection);


		PaginationResult<Course> paginatedCourses = await coursesQuery.PaginateAsync(queryExecutor: _queryExecutor,
																					 paginationRequest: new PaginationQueryRequest
																					 {
																						 PageNumber = query.PaginationRequest.PageNumber,
																						 PageSize = query.PaginationRequest.PageSize
																					 },
																					 cancellationToken: ct);

		GetCourseItemResult[] data = await Task.WhenAll(
			paginatedCourses.Data.Select(async course =>
			{
				Result<string> coverImageFileUrlResult = await _storageService.GetFileUrlAsync(fileReference: new StorageFileReference(StorageProvider: course.CoverImageFile.StorageProvider,
																																	   ContainerName: course.CoverImageFile.ContainerName,
																																	   StoredFileName: course.CoverImageFile.StoredFileName),
																							   urlExpiration: TimeSpan.FromMinutes(10),
																							   cancellationToken: ct);

				return new GetCourseItemResult(
					Id: course.Id,
					Title: course.Title,
					SubTitle: course.SubTitle,
					CategoryType: course.Category.CategoryType.Slug,
					CategoryName: course.Category.Title,
					CategoryId: course.CategoryId,
					CoverImageFileUrl: coverImageFileUrlResult.IsSuccess ? coverImageFileUrlResult.Value : string.Empty,
					DurationSec: course.DurationSec,
					IsFeatured: course.IsFeatured,
					Narrators: course.Tracks.Where(track => !track.IsDeleted && !track.Narrator.IsDeleted)
											.Select(track => track.Narrator.Slug)
											.Distinct()
											.ToList()
				);
			})
		);


		return Result<GetCoursesResult>.Success(
			new GetCoursesResult(
				PaginationResult: new PaginationResult<GetCourseItemResult>
				{
					Data = data,
					Meta = paginatedCourses.Meta
				}
			)
		);
	}

}
