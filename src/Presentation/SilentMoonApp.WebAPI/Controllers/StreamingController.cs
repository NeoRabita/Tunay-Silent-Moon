using Microsoft.AspNetCore.Authorization;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.DTOs.Storage;
using SilentMoonApp.Application.Features.Tracks.GetTrackAudioFile;
using SilentMoonApp.Application.Features.Tracks.GetTrackAudioFileUrl;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.WebAPI.Contracts.Tracks.GetTrackAudioFile;

namespace SilentMoonApp.WebAPI.Controllers;


[Authorize]
[ApiController]
[Route("api/streaming")]
public class StreamingController : BaseController
{
	private static readonly TimeSpan StreamUrlExpiration = TimeSpan.FromMinutes(10);

	private readonly IDispatcher _dispatcher;
	private readonly IStorageService _storageService;
	private readonly IHttpClientFactory _httpClientFactory;
	private readonly TimeProvider _timeProvider;

	public StreamingController(IDispatcher dispatcher,
							   IStorageService storageService,
							   IHttpClientFactory httpClientFactory,
							   TimeProvider timeProvider)
	{
		_dispatcher = dispatcher;
		_storageService = storageService;
		_httpClientFactory = httpClientFactory;
		_timeProvider = timeProvider;
	}




	[HttpGet("{id:guid}/stream-url")]

	[ProducesResponseType(typeof(GetTrackAudioFileUrlResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

	public async Task<IActionResult> GetTrackAudioFileUrl([FromRoute] Guid id,
																	  CancellationToken cancellationToken)
	{
		Result<GetTrackAudioFileUrlResult> audioFileResult = await _dispatcher.SendAsync(query: new GetTrackAudioFileUrlQuery(TrackId: id),
																					  cancellationToken: cancellationToken);

		return HandleResult(
			result: audioFileResult,
			onSuccess: audioFile => Ok(new GetTrackAudioFileUrlResponse
			{
				TrackId = audioFile.TrackId,
				CourseId = audioFile.CourseId,
				TrackTitle = audioFile.TrackTitle,
				FileName = audioFile.FileName,
				StreamUrl = audioFile.StreamUrl,
				ExpiresAt = audioFile.ExpiresAt,
				ContentType = audioFile.ContentType,
				FileSizeBytes = audioFile.FileSizeBytes,
				DurationSec = audioFile.DurationSec
			}
		));
	}



	[AllowAnonymous]
	[HttpGet("{id:guid}/stream")]
	[Produces("audio/mpeg", "audio/wav", "application/octet-stream")]

	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]

	public async Task<IActionResult> GetTrackAudioFileStream([FromRoute] Guid id,
																		 CancellationToken cancellationToken)
	{
		string rangeHeader = Request.Headers.Range.ToString();

		Result<GetTrackAudioFileResult> audioFileResult = await _dispatcher.SendAsync(query: new GetTrackAudioFileQuery(TrackId: id,
																														 RangeHeader: rangeHeader),
																					   cancellationToken: cancellationToken);
		if (audioFileResult.IsFailure)
			return HandleResult(audioFileResult);


		StorageStreamResult streamResult = audioFileResult.Value.StorageStreamResult;

		Response.RegisterForDispose(streamResult);
		Response.StatusCode = streamResult.StatusCode;


		if (streamResult.ContentLength.HasValue)
			Response.ContentLength = streamResult.ContentLength.Value;

		if (streamResult.ContentRange is not null)
			Response.Headers.Append(key: "Content-Range",
									value: streamResult.ContentRange.ToString());

		if (streamResult.AcceptRanges)
			Response.Headers["Accept-Ranges"] = "bytes";


		return new FileStreamResult(fileStream: streamResult.Stream,
									contentType: streamResult.ContentType);
	}
}