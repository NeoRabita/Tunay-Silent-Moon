using Microsoft.AspNetCore.Authorization;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;
using SilentMoonApp.Application.Features.Topics.Commands.UpdateMyTopics;
using SilentMoonApp.Application.Features.Topics.Queries.GetMyTopics;
using SilentMoonApp.Application.Features.Topics.Queries.GetTopics;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.WebAPI.Contracts.Topics.GetMyTopics;
using SilentMoonApp.WebAPI.Contracts.Topics.GetTopics;
using SilentMoonApp.WebAPI.Contracts.Topics.UpdateMyTopics;


namespace SilentMoonApp.WebAPI.Controllers;

[ApiController]
[Route("api/onboarding")]
public class OnboardingController : BaseController
{
	private readonly IDispatcher _dispatcher;

	public OnboardingController(IDispatcher dispatcher)
	{
		_dispatcher = dispatcher;
	}




	[AllowAnonymous]
	[HttpGet("/topics")]

	[ProducesResponseType(typeof(GetTopicsResponse), StatusCodes.Status200OK)]

	public async Task<IActionResult> GetTopics(CancellationToken cancellationToken)
	{
		var query = new GetTopicsQuery();

		Result<IReadOnlyList<GetTopicsResult>> result = await _dispatcher.SendAsync(query: query,
																					cancellationToken: cancellationToken);

		return HandleResult(
			result: result,

			onSuccess: topics => Ok(
				new GetTopicsResponse
				{
					Topics = topics.Select(topic => new GetTopicResponse
					{
						Id = topic.Id,
						Slug = topic.Slug,
						Title = topic.Title,
						IconUrl = topic.IconUrl,
						ColorHex = topic.ColorHex
					}).ToList()
				}
			)
		);
	}



	[Authorize]
	[HttpGet("me/topics")]

	[ProducesResponseType(typeof(GetMyTopicsResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(GetMyTopicsResponse), StatusCodes.Status401Unauthorized)]

	public async Task<IActionResult> GetMyTopics(CancellationToken cancellationToken)
	{
		var query = new GetMyTopicsQuery();
		Result<IReadOnlyList<GetMyTopicsResult>> result = await _dispatcher.SendAsync(query: query,
																					cancellationToken: cancellationToken);
		return HandleResult(
			result: result,
			onSuccess: topics => Ok(
				new GetMyTopicsResponse
				{
					Topics = topics.Select(topic => new GetMyTopicResponse
					{
						Id = topic.Id,
						Slug = topic.Slug,
						Title = topic.Title,
						IconUrl = topic.IconUrl,
						ColorHex = topic.ColorHex
					}).ToList()
				}
			)
		);
	}


	[Authorize]
	[HttpPut("me/topics")]

	[ProducesResponseType(typeof(UpdateMyTopicsResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(UpdateMyTopicsResponse), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(UpdateMyTopicsResponse), StatusCodes.Status422UnprocessableEntity)]

	public async Task<IActionResult> UpdateMyTopics([FromBody] UpdateMyTopicsRequest request,
									 CancellationToken cancellationToken)
	{
		UpdateMyTopicsCommand  command = new UpdateMyTopicsCommand(TopicIds: request.TopicIds);
		
		Result<IReadOnlyList<UpdateMyTopicsResult>> result = await _dispatcher.SendAsync(command: command,
																					  cancellationToken: cancellationToken);
		return HandleResult(
			result: result,
			onSuccess: topics => Ok(
				new UpdateMyTopicsResponse
				{
					Topics = topics.Select(topic => new UpdateMyTopicResponse
					{
						Id = topic.Id,
						Slug = topic.Slug,
						Title = topic.Title,
						IconUrl = topic.IconUrl,
						ColorHex = topic.ColorHex
					}).ToList()
				}
			)
		);
	}

}
