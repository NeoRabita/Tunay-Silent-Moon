using Microsoft.AspNetCore.Authorization;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;
using SilentMoonApp.WebAPI.Contracts.TrackProgresses.CreateMyTrackProgress;
using Microsoft.AspNetCore.Http.HttpResults;
using SilentMoonApp.Application.Features.TrackProgresses.Commands.CreateMyTrackProgress;
using SilentMoonApp.Application.Features.TrackProgresses.Queries.GetMyTrackProgress;
using SilentMoonApp.WebAPI.Contracts.TrackProgresses.GetMyTrackProgress;
using SilentMoonApp.WebAPI.Contracts.TrackProgresses.GetMyTrackProgressHistory;
using SilentMoonApp.Application.Features.TrackProgresses.Queries.GetMyTrackProgressHistory;
using SilentMoonApp.WebAPI.Contracts.Common;
using SilentMoonApp.Application.DTOs.Common;


namespace SilentMoonApp.WebAPI.Controllers;


[Authorize]
[ApiController]
[Route("api/player")]
public class PlayerController : BaseController
{
	private readonly IDispatcher _dispatcher;

	public PlayerController(IDispatcher dispatcher)
	{
		_dispatcher = dispatcher;
	}




	[HttpPost("me/progress")]

	[ProducesResponseType(typeof(CreateMyTrackProgressResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]

	public async Task<IActionResult> CreateMyTrackProgress(CreateMyTrackProgressRequest request,
														   CancellationToken cancellationToken = default)
	{
		CreateMyTrackProgressCommand command = new
		(

			TrackId: request.TrackId,
			PositionSec: request.PositionSec,
			Completed: request.Completed
		);


		Result<CreateMyTrackProgressResult> result = await _dispatcher.SendAsync(command: command,
																				 cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: progress => Ok(new CreateMyTrackProgressResponse
			{
				Id = progress.Id,
				TrackId = progress.TrackId,
				PositionSec = progress.PositionSec,
				Completed = progress.Completed,
				UpdatedAt = progress.UpdatedAt
			})
		);
	}



	[HttpGet("me/progress/{trackId:guid}")]

	[ProducesResponseType(typeof(GetMyTrackProgressResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

	public async Task<IActionResult> GetMyTrackProgress([FromRoute] Guid trackId,
																    CancellationToken cancellationToken = default)
	{
		GetMyTrackProgressQuery query = new
		(
			TrackId: trackId
		);

		Result<GetMyTrackProgressResult> result = await _dispatcher.SendAsync(query: query,
																			  cancellationToken: cancellationToken);

		return HandleResult(result: result,
			onSuccess: progress => Ok(
				value: new GetMyTrackProgressResponse
				{
					Id = progress.Id,
					TrackId = progress.TrackId,
					PositionSec = progress.PositionSec,
					Completed = progress.Completed,
					UpdatedAt = progress.UpdatedAt
				}
			)
		);
	}



	[HttpGet("me/history")]

	[ProducesResponseType(typeof(PaginationResponse<GetMyTrackProgressHistoryItemResponse>), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]

	public async Task<IActionResult> GetMyTrackProgressHistory([FromQuery] GetMyTrackProgressHistoryRequest request,
																	 CancellationToken cancellationToken)
	{
		GetMyTrackProgressHistoryQuery query = new(
			PaginationQueryRequest: new PaginationQueryRequest
			{
				PageNumber = request.PageNumber,
				PageSize = request.PageSize
			}
		);


		Result<GetMyTrackProgressHistoryResult> result = await _dispatcher.SendAsync(query: query,
																					 cancellationToken: cancellationToken);


		return HandleResult(
			result: result,
			onSuccess: history => Ok(new PaginationResponse<GetMyTrackProgressHistoryItemResponse>
			{
				Data = history.PaginationResult.Data.Select(item => new GetMyTrackProgressHistoryItemResponse
				{
					Progress = new GetMyTrackProgressResponse
					{
						Id = item.Progress.Id,
						TrackId = item.Progress.TrackId,
						PositionSec = item.Progress.PositionSec,
						Completed = item.Progress.Completed,
						UpdatedAt = item.Progress.UpdatedAt
					},

					Track = new GetMyTrackProgressHistoryTrackResponse
					{
						Id = item.Track.Id,
						CourseId = item.Track.CourseId,
						Title = item.Track.Title,
						Narrator = item.Track.Narrator,
						DurationSec = item.Track.DurationSec,
						AudioUrl = item.Track.AudioUrl,
						MimeType = item.Track.MimeType,
						FileSizeBytes = item.Track.FileSizeBytes,
						ImageUrl = item.Track.ImageUrl,
						TrackNumber = item.Track.TrackNumber
					}
				}).ToList(),

				Meta = new PaginationResponseMeta
				{
					PageNumber = history.PaginationResult.Meta.PageNumber,
					PageSize = history.PaginationResult.Meta.PageSize,
					TotalPages = history.PaginationResult.Meta.TotalPages,
					TotalCount = history.PaginationResult.Meta.TotalCount
				}
			})
		);
	}

}
