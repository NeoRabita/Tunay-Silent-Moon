using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.Helpers;
using SilentMoonApp.Domain;
using System.Globalization;

namespace SilentMoonApp.Application.Features.Reminders.Commands.UpdateReminder;

public class UpdateReminderCommandHandler : ICommandHandler<UpdateReminderCommand, UpdateReminderResult>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly ICurrentUser _currentUser;
	private readonly TimeProvider _timeProvider;

	public UpdateReminderCommandHandler(IUnitOfWork unitOfWork,
										ICurrentUser currentUser,
										TimeProvider timeProvider)
	{
		_unitOfWork = unitOfWork;
		_currentUser = currentUser;
		_timeProvider = timeProvider;
	}


	public async Task<Result<UpdateReminderResult>> Handle(UpdateReminderCommand command, CancellationToken ct = default)
	{
		if (!_currentUser.IsAuthenticated ||
			_currentUser.UserId is not Guid userId)

			return Result<UpdateReminderResult>.Failure(
				AuthErrors.UnAuthorized());

		Reminder? reminder = await _unitOfWork.ReadRepository<Reminder>()
											  .GetAsync(filter: reminder => reminder.Id == command.Id
																		 && reminder.UserId == userId,
														tracking: true,
														cancellationToken: ct);

		if (reminder is null)
			return Result<UpdateReminderResult>.Failure(
				ReminderErrors.NotFound());

		if(reminder.UserId != userId)
			return Result<UpdateReminderResult>.Failure(
				AuthErrors.UserForbidden());

		if (command.Time is not null)
			reminder.Time = TimeSpan.ParseExact(input: command.Time,
												format: @"hh\:mm",
												formatProvider: CultureInfo.InvariantCulture);

		if (command.DaysOfWeek is not null)
			reminder.DaysOfWeek = TimedOperation.GenerateDaysMask(command.DaysOfWeek);

		if (command.Label is not null)
			reminder.Label = command.Label;

		if (command.IsEnabled.HasValue)
			reminder.IsEnabled = command.IsEnabled.Value;

		reminder.UpdatedBy = userId;
		reminder.UpdatedAt = _timeProvider.GetUtcNow();


		return Result<UpdateReminderResult>.Success(
			new UpdateReminderResult
			(
				Id: reminder.Id,
				Time: reminder.Time.ToString(@"hh\:mm"),
				DaysOfWeek: TimedOperation.DecodeDaysMask(reminder.DaysOfWeek),
				Label: reminder.Label,
				IsEnabled: reminder.IsEnabled,
				CreatedAt: reminder.CreatedAt
			)
		);

	}

}
