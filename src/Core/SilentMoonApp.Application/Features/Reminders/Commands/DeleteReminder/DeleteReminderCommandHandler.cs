using SilentMoonApp.Domain;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.Messaging;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Authentication;


namespace SilentMoonApp.Application.Features.Reminders.Commands.DeleteReminder;

public class DeleteReminderCommandHandler : ICommandHandler<DeleteReminderCommand>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly ICurrentUser _currentUser;

	public DeleteReminderCommandHandler(IUnitOfWork unitOfWork,
										ICurrentUser currentUser)
	{
		_unitOfWork = unitOfWork;
		_currentUser = currentUser;
	}


	public async Task<Result<NoResult>> Handle(DeleteReminderCommand command, CancellationToken ct = default)
	{
		if (!_currentUser.IsAuthenticated ||
			_currentUser.UserId is not Guid userId)

			return Result<NoResult>.Failure(
				AuthErrors.UnAuthorized());


		Reminder? reminder = await _unitOfWork.ReadRepository<Reminder>()
											  .GetAsync(filter: reminder => reminder.Id == command.Id
																		 && reminder.UserId == userId,
																			tracking: true,
																			cancellationToken: ct);

		if (reminder is null)
			return Result<NoResult>.Failure(
				ReminderErrors.NotFound());

		if(reminder.UserId != userId)
			return Result<NoResult>.Failure(
				AuthErrors.UserForbidden());


		_unitOfWork.WriteRepository<Reminder>()
				   .Remove(reminder);


		return Result<NoResult>.Success(NoResult.Value);
	}

}
