using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.Helpers;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Repositories.Read;


namespace SilentMoonApp.Application.Features.Reminders.Queries.GetMyReminders;

public class GetMyReminderQueryHandler : IQueryHandler<GetMyRemindersQuery, IReadOnlyList<GetMyRemindersResult>>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly ICurrentUser _currentUser;
	private readonly TimeProvider _timeProvider;

	public GetMyReminderQueryHandler(IUnitOfWork unitOfWork,
									 ICurrentUser currentUser,
									 TimeProvider timeProvider)
	{
		_unitOfWork = unitOfWork;
		_currentUser = currentUser;
		_timeProvider = timeProvider;
	}


	public async Task<Result<IReadOnlyList<GetMyRemindersResult>>> Handle(GetMyRemindersQuery query, CancellationToken cancellationToken = default)
	{
		if (!_currentUser.IsAuthenticated ||
			_currentUser.UserId is not Guid userId)

			return Result<IReadOnlyList<GetMyRemindersResult>>.Failure(
				AuthErrors.UnAuthorized());


		User? user = await _unitOfWork.Repository<IUserReadRepository>()
									  .GetByIdWithRemindersAsync(userId: userId,
																 tracking: false,
																 cancellationToken: cancellationToken);

		if (user is null || user.IsDeleted)
			return Result<IReadOnlyList<GetMyRemindersResult>>.Failure(
				AuthErrors.UnAuthorized());


		return Result<IReadOnlyList<GetMyRemindersResult>>.Success(
			user.Reminders.Where(reminder => reminder.UserId == userId)
						  .Select(reminder => new GetMyRemindersResult
					      (
							  Id: reminder.Id,
							  Time: reminder.Time.ToString(@"hh\:mm"),
							  DaysOfWeek: TimedOperation.DecodeDaysMask(reminder.DaysOfWeek),
							  Label: reminder.Label,
							  IsEnabled: reminder.IsEnabled,
							  CreatedAt: reminder.CreatedAt
					      )
			).ToList()
		);
	}

}