using Microsoft.AspNetCore.Authorization;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;
using SilentMoonApp.Application.DTOs.Common;
using SilentMoonApp.Application.Features.Categories.Queries.GetCategories;
using SilentMoonApp.Application.Features.Courses.Queries.GetCourseDetail;
using SilentMoonApp.Application.Features.Courses.Queries.GetCourses;
using SilentMoonApp.Application.Features.Courses.Queries.GetCourseTrackById;
using SilentMoonApp.Application.Features.Courses.Queries.GetCourseTracks;
using SilentMoonApp.Application.Features.Courses.Queries.GetCourseWithNarrators;
using SilentMoonApp.Application.Features.Courses.Queries.GetRelatedCourses;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.WebAPI.Contracts.Categories.GetCategories;
using SilentMoonApp.WebAPI.Contracts.Common;
using SilentMoonApp.WebAPI.Contracts.Courses.GetCourseDetail;
using SilentMoonApp.WebAPI.Contracts.Courses.GetCourses;
using SilentMoonApp.WebAPI.Contracts.Courses.GetCourseTrackById;
using SilentMoonApp.WebAPI.Contracts.Courses.GetCourseTracks;
using SilentMoonApp.WebAPI.Contracts.Courses.GetCourseWithNarrators;
using SilentMoonApp.WebAPI.Contracts.Courses.GetRelatedCourses;


namespace SilentMoonApp.WebAPI.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController : BaseController
{
	private readonly IDispatcher _dispatcher;

	public CatalogController(IDispatcher dispatcher)
	{
		_dispatcher = dispatcher;
	}




	[AllowAnonymous]
	[HttpGet("categories")]

	[ProducesResponseType(typeof(GetCategoriesResponse), StatusCodes.Status200OK)]

	public async Task<IActionResult> GetCategories([FromQuery] string? type, CancellationToken cancellationToken)
	{
		GetCategoriesQuery query = new
		(
			Type: type
		);

		Result<IReadOnlyList<GetCategoriesResult>> result = await _dispatcher.SendAsync(query: query,
																						cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: categories => Ok(
				new GetCategoriesResponse
				{
					Categories = categories.Select(category => new GetCategoryResponse
					{
						Id = category.Id,
						Title = category.Title,
						Slug = category.Slug,
						Type = category.Type,
						IconUrl = category.IconUrl ?? string.Empty
					}).ToList()
				}
			)
		);
	}




	[AllowAnonymous]
	[HttpGet("courses")]

	[ProducesResponseType(typeof(PaginationResponse<GetCourseItemResponse>), StatusCodes.Status200OK)]

	public async Task<IActionResult> GetCourses([FromQuery] GetCoursesRequest request,
															CancellationToken cancellationToken)
	{
		GetCoursesQuery query = new
		(
			PaginationRequest: new PaginationQueryRequest
			{
				PageNumber = request.PageNumber,
				PageSize = request.PageSize
			},
			CourseSortBy: request.CourseSortBy,
			SortDirection: request.SortDirection,
			CategoryType: request.CategoryType,
			CategoryId: request.CategoryId,
			Search: request.Search,
			IsFeatured: request.IsFeatured
		);


		Result<GetCoursesResult> result = await _dispatcher.SendAsync(query: query,
																	  cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: courses => Ok(
				new PaginationResponse<GetCourseItemResponse>
				{
					Data = courses.PaginationResult.Data.Select(course => new GetCourseItemResponse
					{
						Id = course.Id,
						Title = course.Title,
						SubTitle = course.SubTitle,
						CategoryType = course.CategoryType,
						CategoryName = course.CategoryName,
						CategoryId = course.CategoryId,
						ImageUrl = course.CoverImageFileUrl ?? string.Empty,
						DurationSec = course.DurationSec,
						IsFeatured = course.IsFeatured,
						Narrators = course.Narrators
					}).ToList(), 

					Meta = new PaginationResponseMeta
					{
						PageNumber = courses.PaginationResult.Meta.PageNumber,
						PageSize = courses.PaginationResult.Meta.PageSize,
						TotalCount = courses.PaginationResult.Meta.TotalCount,
						TotalPages = courses.PaginationResult.Meta.TotalPages
					}
				}

			)
		);
	}




	[AllowAnonymous]
	[HttpGet("courses/{id:guid}")]

	[ProducesResponseType(typeof(GetCourseWithNarratorsResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

	public async Task<IActionResult> GetCourseWithNarrators([FromRoute] Guid id,
														CancellationToken cancellationToken)
	{
		GetCourseWithNarratorsQuery query = new
		(
			Id: id
		);

		Result<GetCourseWithNarratorsResult> result = await _dispatcher.SendAsync(query: query,
																				  cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: course => Ok(
				new GetCourseWithNarratorsResponse
				{
					Id = course.Id,
					Title = course.Title,
					SubTitle = course.SubTitle,
					CategoryType = course.CategoryType,
					CategoryId = course.CategoryId,
					ImageUrl = course.ImageUrl,
					DurationSec = course.DurationSec,
					IsFeatured = course.IsFeatured,
					Narrators = course.Narrators,
					Description = course.Description,
					TrackCount = course.TrackCount,
					CreatedAt = course.CreatedAt
				}
			)
		);
	}




	[Authorize]
	[HttpGet("courses/{id:guid}/detail")]

	[ProducesResponseType(typeof(GetCourseDetailResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

	public async Task<IActionResult> GetCourseDetail([FromRoute] Guid id,
													 [FromQuery] GetCourseDetailRequest request,
																 CancellationToken cancellationToken)
	{
		GetCourseDetailQuery query = new
		(
			Id: id,
			NarratorId: request.NarratorId
		);

		Result<GetCourseDetailResult> result = await _dispatcher.SendAsync(query: query,
																		   cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: detail => Ok(
				new GetCourseDetailResponse
				{
					Course = new GetCourseDetailCourseResponse
					{
						Id = detail.Course.Id,
						Title = detail.Course.Title,
						SubTitle = detail.Course.SubTitle,
						CategoryType = detail.Course.CategoryType,
						CategoryId = detail.Course.CategoryId,
						ImageUrl = detail.Course.ImageUrl,
						DurationSec = detail.Course.DurationSec,
						IsFeatured = detail.Course.IsFeatured,
						Narrators = detail.Course.Narrators.Select(narrator => new GetCourseDetailNarratorResponse
						{
							Id = narrator.Id,
							Name = narrator.Name,
							Slug = narrator.Slug
						}).ToList(),
						Description = detail.Course.Description,
						TrackCount = detail.Course.TrackCount,
						CreatedAt = detail.Course.CreatedAt
					},

					Tracks = detail.Tracks.Select(track => new GetCourseDetailTrackResponse
					{
						Id = track.Id,
						CourseId = track.CourseId,
						Title = track.Title,
						NarratorId = track.NarratorId,
						NarratorName = track.NarratorName,
						NarratorSlug = track.NarratorSlug,
						DurationSec = track.DurationSec,
						AudioUrl = track.AudioUrl,
						MimeType = track.MimeType,
						FileSizeBytes = track.FileSizeBytes,
						ImageUrl = track.ImageUrl,
						TrackNumber = track.TrackNumber
					}).ToList(),

					UserProgress = detail.UserProgress is null
						? null
						: new GetCourseDetailUserProgressResponse
						{
							Id = detail.UserProgress.Id,
							TrackId = detail.UserProgress.TrackId,
							PositionSec = detail.UserProgress.PositionSec,
							Completed = detail.UserProgress.Completed,
							UpdatedAt = detail.UserProgress.UpdatedAt
						},

					IsFavorited = detail.IsFavorited
				}
			)
		);
	}




	[AllowAnonymous]
	[HttpGet("courses/{id:guid}/related")]

	[ProducesResponseType(typeof(GetRelatedCoursesResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

	public async Task<IActionResult> GetRelatedCourses([FromRoute] Guid id,
													   [FromQuery] GetRelatedCoursesRequest request,
																   CancellationToken cancellationToken)
	{
		GetRelatedCoursesQuery query = new
		(
			Id: id,
			Limit: request.Limit
		);

		Result<IReadOnlyList<GetRelatedCoursesResult>> result = await _dispatcher.SendAsync(query: query,
																							cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: courses => Ok(
				new GetRelatedCoursesResponse
				{
					Courses = courses.Select(course => new GetRelatedCourseItemResponse
					{
						Id = course.Id,
						Title = course.Title,
						SubTitle = course.SubTitle,
						Type = course.Type,
						CategoryId = course.CategoryId,
						ImageUrl = course.ImageUrl,
						DurationSec = course.DurationSec,
						IsFeatured = course.IsFeatured,
						Narrators = course.Narrators
					}).ToList()
				}
			)
		);
	}




	[AllowAnonymous]
	[HttpGet("courses/{id:guid}/tracks")]

	[ProducesResponseType(typeof(GetCourseTracksResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

	public async Task<IActionResult> GetCourseTracks([FromRoute] Guid id,
													 [FromQuery] GetCourseTracksRequest request,
																 CancellationToken cancellationToken)
	{
		GetCourseTracksQuery query = new
		(
			Id: id,
			NarratorId: request.NarratorId
		);

		Result<GetCourseTracksResult> result = await _dispatcher.SendAsync(query: query,
																		   cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: tracks => Ok(
				new GetCourseTracksResponse
				{
					Tracks = tracks.Tracks.Select(track => new GetCourseTrackResponse
					{
						Id = track.Id,
						CourseId = track.CourseId,
						Title = track.Title,
						NarratorId = track.NarratorId,
						NarratorName = track.NarratorName,
						NarratorSlug = track.NarratorSlug,
						DurationSec = track.DurationSec,
						AudioUrl = track.AudioUrl,
						MimeType = track.MimeType,
						FileSizeBytes = track.FileSizeBytes,
						ImageUrl = track.ImageUrl,
						TrackNumber = track.TrackNumber
					}).ToList()
				}
			)
		);
	}




	[AllowAnonymous]
	[HttpGet("tracks/{id:guid}")]

	[ProducesResponseType(typeof(GetCourseTrackByIdResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

	public async Task<IActionResult> GetTrack([FromRoute] Guid id,
													      CancellationToken cancellationToken)
	{
		GetCourseTrackByIdQuery query = new(Id: id);

		Result<GetCourseTrackByIdResult> result = await _dispatcher.SendAsync(query: query,
																			  cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: track => Ok(
				new GetCourseTrackByIdResponse
				{
					Id = track.Id,
					CourseId = track.CourseId,
					Title = track.Title,
					NarratorId = track.NarratorId,
					NarratorName = track.NarratorName,
					NarratorSlug = track.NarratorSlug,
					DurationSec = track.DurationSec,
					AudioUrl = track.AudioUrl,
					MimeType = track.MimeType,
					FileSizeBytes = track.FileSizeBytes,
					ImageUrl = track.ImageUrl,
					TrackNumber = track.TrackNumber
				}
			)
		);
	}

}