using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Domain.Entities;

namespace SilentMoonApp.Application.Features.TrackProgresses.Commands.CreateMyTrackProgress;

public class CreateMyTrackProgressCommandHandler : ICommandHandler<CreateMyTrackProgressCommand, CreateMyTrackProgressResult>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly ICurrentUser _currentUser;
	private readonly TimeProvider _timeProvider;

	public CreateMyTrackProgressCommandHandler(IUnitOfWork unitOfWork,
											   ICurrentUser currentUser,
											   TimeProvider timeProvider)
	{
		_unitOfWork = unitOfWork;
		_currentUser = currentUser;
		_timeProvider = timeProvider;
	}


	public async Task<Result<CreateMyTrackProgressResult>> Handle(CreateMyTrackProgressCommand command,
																   CancellationToken ct = default)
	{
		if (!_currentUser.IsAuthenticated || _currentUser.UserId is not Guid userId)
			return Result<CreateMyTrackProgressResult>.Failure(
				AuthErrors.UnAuthorized());

		User? user = await _unitOfWork.ReadRepository<User>()
									  .GetByIdAsync(id: userId,
													tracking: false,
													cancellationToken: ct);
		if (user is null || user.IsDeleted)
			return Result<CreateMyTrackProgressResult>.Failure(
				AuthErrors.UnAuthorized());


		Track? track = await _unitOfWork.Repository<ITrackReadRepository>()
			.GetTrackDetailAsync(
				id: command.TrackId,
				tracking: false,
				cancellationToken: ct);

		if (track is null)
			return Result<CreateMyTrackProgressResult>.Failure(
				TrackProgressErrors.NotFound());

		int durationSec = track.AudioFile.DurationSec;

		if (durationSec > 0 && command.PositionSec > durationSec)
			return Result<CreateMyTrackProgressResult>.Failure(
				TrackProgressErrors.InvalidPosition(durationSec));

		int positionSec = command.Completed
			? durationSec
			: command.PositionSec;


		TrackProgress? trackProgress = await _unitOfWork.ReadRepository<TrackProgress>()
														.GetAsync(filter: tp => tp.UserId == userId
													 						 && tp.TrackId == command.TrackId,
													 			  tracking: true,
													 			  cancellationToken: ct);

		DateTimeOffset updatedAt = _timeProvider.GetUtcNow();

		if (trackProgress is null)
		{
			trackProgress = new TrackProgress
			{
				UserId = userId,
				TrackId = command.TrackId,
				PositionSec = positionSec,
				Completed = command.Completed,
				UpdatedAt = updatedAt
			};

			await _unitOfWork.WriteRepository<TrackProgress>()
							 .AddAsync(entity: trackProgress, cancellationToken: ct);
		}

		else
		{
			trackProgress.PositionSec = positionSec;
			trackProgress.Completed = command.Completed;
			trackProgress.UpdatedAt = updatedAt;
		}


		return Result<CreateMyTrackProgressResult>.Success
		(
			new CreateMyTrackProgressResult
			(
				Id : trackProgress.Id,
				TrackId : trackProgress.TrackId,
				PositionSec : trackProgress.PositionSec,			
				Completed : trackProgress.Completed,
				UpdatedAt : trackProgress.UpdatedAt
			)
		);
	}

}
