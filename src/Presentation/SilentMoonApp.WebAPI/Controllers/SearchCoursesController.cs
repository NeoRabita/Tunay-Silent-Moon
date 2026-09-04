using MailKit.Search;
using Microsoft.AspNetCore.Authorization;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;
using SilentMoonApp.Application.DTOs.Common;
using SilentMoonApp.Application.Features.SearchCourses.Queries.GetSearchCourses;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.WebAPI.Contracts.Common;
using SilentMoonApp.WebAPI.Contracts.SearchCourses.GetSearchCourses;

namespace SilentMoonApp.WebAPI.Controllers;


[ApiController]
[Route("api")]
public class SearchCoursesController : BaseController
{
	private readonly IDispatcher _dispatcher;

	public SearchCoursesController(IDispatcher dispatcher)
	{
		_dispatcher = dispatcher;
	}


	[AllowAnonymous]
	[HttpGet("search")]

	[ProducesResponseType(typeof(GetSearchCoursesResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]

	public async Task<IActionResult> Search(
		[FromQuery] GetSearchCoursesRequest request,
		CancellationToken cancellationToken)
	{
		GetSearchCoursesQuery query = new
		(
			PaginationQueryRequest: new PaginationQueryRequest
			{
				PageNumber = request.PageNumber,
				PageSize = request.PageSize
			},

			Search: request.Search,
			CategoryTypeId: request.CategoryTypeId
		);

		Result<GetSearchCoursesResult> result = await _dispatcher.SendAsync(query: query,
																			cancellationToken: cancellationToken);

		return HandleResult
		(
			result: result,
			onSuccess: searchResult => Ok(new GetSearchCoursesResponse
			{
				Search = searchResult.Search,

				Data = searchResult.PaginationResult.Data.Select(course => new GetSearchCourseItemResponse
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
				}).ToList(),

				Meta = new PaginationResponseMeta
				{
					PageNumber = searchResult.PaginationResult.Meta.PageNumber,
					PageSize = searchResult.PaginationResult.Meta.PageSize,
					TotalCount = searchResult.PaginationResult.Meta.TotalCount,
					TotalPages = searchResult.PaginationResult.Meta.TotalPages
				}
			})
		);
	}
}
