using Microsoft.AspNetCore.Authorization;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;
using SilentMoonApp.Application.DTOs.Common;
using SilentMoonApp.Application.Features.CourseFavorites.Commands.CreateMyFavorite;
using SilentMoonApp.Application.Features.CourseFavorites.Commands.DeleteMyFavorite;
using SilentMoonApp.Application.Features.CourseFavorites.Queries.GetMyFavorites;
using SilentMoonApp.Application.Messaging;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.WebAPI.Contracts.Common;
using SilentMoonApp.WebAPI.Contracts.CourseFavorites.CreateMyFavorite;
using SilentMoonApp.WebAPI.Contracts.CourseFavorites.GetMyFavorites;


namespace SilentMoonApp.WebAPI.Controllers;

[Authorize]
[ApiController]
[Route("api/favorites")]
public class FavoritesController : BaseController
{
	private readonly IDispatcher _dispatcher;

	public FavoritesController(IDispatcher dispatcher)
	{
		_dispatcher = dispatcher;
	}



	[HttpPost("me/favorites")]

	[ProducesResponseType(typeof(CreateMyFavoriteResponse), StatusCodes.Status201Created)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]

	public async Task<IActionResult> CreateMyFavorite([FromBody] CreateMyFavoriteRequest request,
																CancellationToken cancellationToken = default)
	{
		CreateMyFavoriteCommand command = new
		(
			CourseId: request.CourseId
		);

		Result<CreateMyFavoriteResult> result = await _dispatcher.SendAsync(command: command,
																			cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: favorite => Created(
				uri: $"/api/favorites/me/favorites/{favorite.Id}",
				value: new CreateMyFavoriteResponse
				{
					Id = favorite.Id,
					CourseId = favorite.CourseId,

					Course = new CreateMyFavoriteCourseResponse
					{
						Id = favorite.Course.Id,
						Title = favorite.Course.Title,
						SubTitle = favorite.Course.SubTitle,
						CategoryTypeId = favorite.Course.CategoryTypeId,
						CategoryId = favorite.Course.CategoryId,
						ImageUrl = favorite.Course.ImageUrl,
						DurationSec = favorite.Course.DurationSec,
						IsFeatured = favorite.Course.IsFeatured,
						Narrators = favorite.Course.Narrators
					},

					CreatedAt = favorite.CreatedAt
				})
		);
	}



	[HttpGet("me/favorites")]

	[ProducesResponseType(typeof(GetMyFavoritesResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]

	public async Task<IActionResult> GetMyFavorites([FromQuery] GetMyFavoritesRequest request,
																CancellationToken cancellationToken = default)
	{
		GetMyFavoritesQuery query = new
		(
			PaginationQueryRequest: new PaginationQueryRequest
			{
				PageNumber = request.PageNumber,
				PageSize = request.PageSize
			},

			CategoryTypeId: request.CategoryTypeId
		);

		Result<GetMyFavoritesResult> result = await _dispatcher.SendAsync(query: query,
																		  cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: favorites => Ok(new GetMyFavoritesResponse
			{
				Data = favorites.PaginationResult.Data.Select(favorite => new GetMyFavoriteItemResponse
				{
					Id = favorite.Id,
					CourseId = favorite.CourseId,

					Course = new GetMyFavoriteCourseResponse
					{
						Id = favorite.Course.Id,
						Title = favorite.Course.Title,
						SubTitle = favorite.Course.SubTitle,
						CategoryTypeId = favorite.Course.CategoryTypeId,
						CategoryId = favorite.Course.CategoryId,
						ImageUrl = favorite.Course.ImageUrl,
						DurationSec = favorite.Course.DurationSec,
						IsFeatured = favorite.Course.IsFeatured,
						Narrators = favorite.Course.Narrators
					},

					CreatedAt = favorite.CreatedAt
				}).ToList(),

				Meta = new PaginationResponseMeta
				{
					PageNumber = favorites.PaginationResult.Meta.PageNumber,
					PageSize = favorites.PaginationResult.Meta.PageSize,
					TotalCount = favorites.PaginationResult.Meta.TotalCount,
					TotalPages = favorites.PaginationResult.Meta.TotalPages
				}
			})
		);

	}


	[HttpDelete("me/favorites/{courseId:guid}")]

	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

	public async Task<IActionResult> DeleteMyFavorite([FromRoute] Guid courseId,
																  CancellationToken cancellationToken = default)
	{
		DeleteMyFavoriteCommand command = new
		(
			CourseId: courseId
		);

		Result<NoResult> result = await _dispatcher.SendAsync(command: command,
															  cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: _ => NoContent()
		);
	}

}
