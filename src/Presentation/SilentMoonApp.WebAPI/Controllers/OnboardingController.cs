using Microsoft.AspNetCore.Authorization;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;
using SilentMoonApp.Application.Features.Reminders.Commands.CreateReminder;
using SilentMoonApp.Application.Features.Reminders.Commands.DeleteReminder;
using SilentMoonApp.Application.Messaging;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.WebAPI.Contracts.Topics.GetTopics;
using SilentMoonApp.WebAPI.Contracts.Topics.GetMyTopics;
using SilentMoonApp.WebAPI.Contracts.Topics.UpdateMyTopics;
using SilentMoonApp.WebAPI.Contracts.Reminders.CreateReminder;
using SilentMoonApp.WebAPI.Contracts.Reminders.DeleteReminder;
using SilentMoonApp.WebAPI.Contracts.Reminders.UpdateReminder;
using SilentMoonApp.Application.Features.Topics.Queries.GetTopics;
using SilentMoonApp.Application.Features.Topics.Queries.GetMyTopics;
using SilentMoonApp.Application.Features.Topics.Commands.UpdateMyTopics;
using SilentMoonApp.Application.Features.Reminders.Queries.GetMyReminders;
using SilentMoonApp.Application.Features.Reminders.Commands.UpdateReminder;
using SilentMoonApp.WebAPI.Contracts.Reminders.GetMyReminders;


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
	[HttpGet("topics")]

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
		GetMyTopicsQuery query = new();

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
		UpdateMyTopicsCommand command = new UpdateMyTopicsCommand(TopicIds: request.TopicIds);

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



	[Authorize]
	[HttpGet("me/reminders")]

	[ProducesResponseType(typeof(GetMyReminderResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(GetMyReminderResponse), StatusCodes.Status401Unauthorized)]

	public async Task<IActionResult> GetMyReminders(CancellationToken cancellationToken)
	{
		GetMyRemindersQuery query = new GetMyRemindersQuery();

		Result<IReadOnlyList<GetMyRemindersResult>> result = await _dispatcher.SendAsync(query: query,
																						 cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: reminders => Ok(
				new GetMyRemindersResponse
				{
					Reminders = reminders.Select(reminder => new GetMyReminderResponse
					{
						Id = reminder.Id,
						Time = reminder.Time,
						DaysOfWeek = reminder.DaysOfWeek,
						Label = reminder.Label,
						IsEnabled = reminder.IsEnabled,
						CreatedAt = reminder.CreatedAt
					}).ToList()
				}
			)
		);
	}



	[Authorize]
	[HttpPost("me/reminders")]

	[ProducesResponseType(typeof(CreateReminderResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(CreateReminderResponse), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(CreateReminderResponse), StatusCodes.Status422UnprocessableEntity)]

	public async Task<IActionResult> CreateReminder(CreateReminderRequest request,
													CancellationToken ct)
	{
		CreateReminderCommand command = new
		(
			Time: request.Time,
			DaysOfWeek: request.DaysOfWeek,
			Label: request.Label
		);


		Result<CreateReminderResult> result = await _dispatcher.SendAsync(command: command,
																		  cancellationToken: ct);

		return HandleResult(
			result: result,
			onSuccess: reminder => Ok(
				new CreateReminderResponse
				{
					Id = reminder.Id,
					Time = reminder.Time,
					DaysOfWeek = reminder.DaysOfWeek,
					Label = reminder.Label,
					IsEnabled = reminder.IsEnabled,
					CreatedAt = reminder.CreatedAt
				}
			)
		);
	}



	[Authorize]
	[HttpPatch("me/reminders/{id}")]

	[ProducesResponseType(typeof(UpdateReminderResponse), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(UpdateReminderResponse), StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(typeof(UpdateReminderResponse), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(UpdateReminderResponse), StatusCodes.Status404NotFound)]
	[ProducesResponseType(typeof(UpdateReminderResponse), StatusCodes.Status422UnprocessableEntity)]

	public async Task<IActionResult> UpdateReminder(Guid id, UpdateReminderRequest request,
													 CancellationToken cancellationToken)
	{
		UpdateReminderCommand command = new
		(
			Id: id,
			Time: request.Time,
			DaysOfWeek: request.DaysOfWeek,
			Label: request.Label,
			IsEnabled: request.IsEnabled
		);


		Result<UpdateReminderResult> result = await _dispatcher.SendAsync(command: command,
																		  cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: reminder => Ok(
				new UpdateReminderResponse
				{
					Id = reminder.Id,
					Time = reminder.Time,
					DaysOfWeek = reminder.DaysOfWeek,
					Label = reminder.Label,
					IsEnabled = reminder.IsEnabled,
					CreatedAt = reminder.CreatedAt
				}
			)
		);
	}



	[Authorize]
	[HttpDelete("me/reminders/{id}")]

	[ProducesResponseType(typeof(DeleteReminderResponse), StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(DeleteReminderResponse), StatusCodes.Status403Forbidden)]
	[ProducesResponseType(typeof(DeleteReminderResponse), StatusCodes.Status404NotFound)]

	public async Task<IActionResult> DeleteReminder(Guid id, CancellationToken cancellationToken)
	{
		DeleteReminderCommand command = new(Id: id);

		Result<NoResult> result = await _dispatcher.SendAsync(command: command,
															  cancellationToken: cancellationToken);

		return HandleResult(
			result: result,
			onSuccess: _ => NoContent()
		);
	}

}