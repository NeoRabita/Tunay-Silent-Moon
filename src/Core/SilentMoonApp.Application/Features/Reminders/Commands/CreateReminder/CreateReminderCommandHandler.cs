using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.Helpers;
using SilentMoonApp.Domain;
using System.Globalization;

namespace SilentMoonApp.Application.Features.Reminders.Commands.CreateReminder;

public class CreateReminderCommandHandler : ICommandHandler<CreateReminderCommand, CreateReminderResult>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly ICurrentUser _currentUser;
	private readonly TimeProvider _timeProvider;

	public CreateReminderCommandHandler(IUnitOfWork unitOfWork,
										ICurrentUser currentUser,
										TimeProvider timeProvider)
	{
		_unitOfWork = unitOfWork;
		_currentUser = currentUser;
		_timeProvider = timeProvider;
	}


	public async Task<Result<CreateReminderResult>> Handle(CreateReminderCommand command, CancellationToken cancellationToken = default)
	{
		if (!_currentUser.IsAuthenticated ||
			_currentUser.UserId is not Guid userId)

			return Result<CreateReminderResult>.Failure(
				AuthErrors.UnAuthorized());

		User? user = await _unitOfWork.ReadRepository<User>().GetByIdAsync(id: userId,
																		   tracking: false,
																		   cancellationToken: cancellationToken);

		if (user is null || user.IsDeleted)
			return Result<CreateReminderResult>.Failure(
				AuthErrors.UnAuthorized());


		TimeSpan time = TimeSpan.ParseExact(input: command.Time,
											format: @"hh\:mm",
											formatProvider: CultureInfo.InvariantCulture);

		DateTimeOffset nowUtc = _timeProvider.GetUtcNow();


		Reminder reminder = new()
		{
			Time = time,
			DaysOfWeek = TimedOperation.GenerateDaysMask(command.DaysOfWeek),
			Label = command.Label,
			IsEnabled = true,

			CreatedAt = nowUtc,
			CreatedBy = userId,

			UserId = userId,
		};


		await _unitOfWork.WriteRepository<Reminder>().AddAsync(entity: reminder,
															   cancellationToken: cancellationToken);

		return Result<CreateReminderResult>.Success(
			new CreateReminderResult
			(
				Id: reminder.Id,
				Time: reminder.Time.ToString(format:@"hh\:mm"),
				DaysOfWeek: command.DaysOfWeek,
				Label: reminder.Label,
				IsEnabled: reminder.IsEnabled,
				CreatedAt: reminder.CreatedAt
			)
		);
	}

}
