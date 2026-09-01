using SilentMoonApp.Domain.Entities;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.Abstractions.Executors;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.SharedKernel.Resources;


namespace SilentMoonApp.Application.Features.Home.Queries.GetHome;

public class GetHomeQueryHandler : IQueryHandler<GetHomeQuery, GetHomeResult>
{
	public const int SectionLimit = 4;

	private readonly IUnitOfWork _unitOfWork;
	private readonly ICurrentUser _currentUser;
	private readonly TimeProvider _timeProvider;
	private readonly IQueryExecutor _queryExecutor;
	private readonly IStorageService _storageService;

	public GetHomeQueryHandler(IUnitOfWork unitOfWork,
							   ICurrentUser currentUser,
							   TimeProvider timeProvider,
							   IQueryExecutor queryExecutor,
							   IStorageService storageService)
	{
		_unitOfWork = unitOfWork;
		_currentUser = currentUser;
		_timeProvider = timeProvider;
		_queryExecutor = queryExecutor;
		_storageService = storageService;
	}


	public async Task<Result<GetHomeResult>> Handle(GetHomeQuery query,
													CancellationToken ct = default)
	{
		if (!_currentUser.IsAuthenticated || _currentUser.UserId is not Guid userId)
			return Result<GetHomeResult>.Failure(
				AuthErrors.UnAuthorized());

		User? user = await _unitOfWork.Repository<IUserReadRepository>()
									  .GetByIdWithTopicsAsync(userId: userId,
															  tracking: false,
															  cancellationToken: ct);

		if (user is null || user.IsDeleted)
			return Result<GetHomeResult>.Failure(
				AuthErrors.UnAuthorized());


		GetHomeGreetingResult greeting = GenerateGreeting(user);


		IReadOnlyList<string> topicSlugs = user.UserTopics.Where(userTopic => !userTopic.Topic.IsDeleted)
														  .Select(userTopic => userTopic.Topic.Slug.ToLowerInvariant())
														  .Distinct()
														  .ToList();


		IQueryable<Course> courseQuery = _unitOfWork.Repository<ICourseReadRepository>()
													.QueryForList(filter: course => !course.IsDeleted
																				 && !course.Category.IsDeleted
																				 && !course.Category.CategoryType.IsDeleted
																				 && course.Tracks.Any(track => !track.IsDeleted
																											&& !track.AudioFile.IsDeleted),
																  tracking: false);

		IQueryable<Course> recommendedCoursesQuery = courseQuery.Where(course => course.IsRecommended
																			  && (topicSlugs.Count == 0 ||
																				  topicSlugs.Contains(course.Category.Slug) ||
																				  topicSlugs.Contains(course.Category.CategoryType.Slug)))
																.OrderByDescending(course => course.IsFeatured)
																.ThenByDescending(course => course.CreatedAt)
																.Take(SectionLimit);


		IQueryable<Course> dailyThoughtCourseQuery = courseQuery.Where(course => course.IsDailyThought)
																 .OrderByDescending(course => course.UpdatedAt ?? course.CreatedAt)
																 .Take(1);


		IQueryable<Course> featuredCoursesQuery = courseQuery.Where(course => course.IsFeatured)
															 .OrderByDescending(course => course.CreatedAt)
															 .Take(SectionLimit);


		IQueryable<Course> popularCoursesQuery = courseQuery.Where(course => course.IsPopular)
															 .OrderByDescending(course => course.CourseFavorites.Count)
															 .ThenByDescending(course => course.CreatedAt)
															 .Take(SectionLimit);


		IReadOnlyList<Course> recommendedCourses = await _queryExecutor.ToListAsync(query: recommendedCoursesQuery,
																					cancellationToken: ct);

		Course? dailyThoughtCourse = (await _queryExecutor.ToListAsync(query: dailyThoughtCourseQuery,
																	  cancellationToken: ct))
														  .FirstOrDefault();

		IReadOnlyList<Course> featuredCourses = await _queryExecutor.ToListAsync(query: featuredCoursesQuery,
																				 cancellationToken: ct);

		IReadOnlyList<Course> popularCourses = await _queryExecutor.ToListAsync(query: popularCoursesQuery,
																				cancellationToken: ct);


		List<Course> allCourse = recommendedCourses.Concat(featuredCourses)
												   .Concat(popularCourses)
												   .Concat(dailyThoughtCourse is null ? []
																					  : [dailyThoughtCourse])
												   .DistinctBy(course => course.Id)
												   .ToList();


		GetHomeCourseSectionResult[] mappedCourse = await Task.WhenAll(
			allCourse.Select(async course =>
			{
				string imageUrl = string.Empty;

				if (course.CoverImageFile is not null &&
				  !course.CoverImageFile.IsDeleted)
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



				return new GetHomeCourseSectionResult(
						Id: course.Id,
						Title: course.Title,
						SubTitle: course.SubTitle,
						CategoryType: course.Category.CategoryType.Slug,
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


		Dictionary<Guid, GetHomeCourseSectionResult> courseItems = mappedCourse.ToDictionary(course => course.Id);


		return Result<GetHomeResult>.Success(
			new GetHomeResult
			(
				Greeting: greeting,

				Recommended: new GetHomeSectionResult(
					Title: GeneralResources.HomeRecommended,
					Courses: recommendedCourses.Select(course => courseItems[course.Id])
											   .ToList()
				),

				DailyThought: new GetHomeSectionResult(
					Title: GeneralResources.HomeDailyThought,
					Courses: dailyThoughtCourse is null
						? []
						: [courseItems[dailyThoughtCourse.Id]]
				),



				FeaturedSleep: new GetHomeSectionResult(
					Title: GeneralResources.HomeFeaturedSleep,
					Courses: featuredCourses.Select(course => courseItems[course.Id])
											.ToList()
				),

				PopularMeditations: new GetHomeSectionResult(
					Title: GeneralResources.HomePopularMeditations,
					Courses: popularCourses.Select(course => courseItems[course.Id])
										   .ToList()
				)
			)
		);
	}



	// Helpers 

	private GetHomeGreetingResult GenerateGreeting(User? user)
	{
		string userName = string.IsNullOrWhiteSpace(user?.FirstName)
			? GeneralResources.DefaultUserName
			: user.UserName ?? user.FirstName;

		int hour = _timeProvider.GetLocalNow().Hour;

		(string title, string message) = hour switch
		{
			>= 5 and < 12 => (GeneralResources.HomeGreetingMorning,
							  GeneralResources.HomeGreetingMorningMessage),

			>= 12 and < 17 => (GeneralResources.HomeGreetingAfternoon,
							   GeneralResources.HomeGreetingAfternoonMessage),

			>= 17 and < 21 => (GeneralResources.HomeGreetingEvening,
							   GeneralResources.HomeGreetingEveningMessage),

			_ => (GeneralResources.HomeGreetingNight,
				  GeneralResources.HomeGreetingNightMessage)
		};


		return new GetHomeGreetingResult(
			Title: $"{title}, {userName}",
			Message: message
		);
	}
}
