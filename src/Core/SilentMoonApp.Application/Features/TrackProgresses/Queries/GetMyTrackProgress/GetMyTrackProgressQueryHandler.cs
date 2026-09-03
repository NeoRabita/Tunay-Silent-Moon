using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.Features.TrackProgresses.Commands.CreateMyTrackProgress;
using SilentMoonApp.Domain.Entities;

namespace SilentMoonApp.Application.Features.TrackProgresses.Queries.GetMyTrackProgress;

public class GetMyTrackProgressQueryHandler : IQueryHandler<GetMyTrackProgressQuery, GetMyTrackProgressResult>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly ICurrentUser _currentUser;

	public GetMyTrackProgressQueryHandler(IUnitOfWork unitOfWork,
										  ICurrentUser currentUser)
	{
		_unitOfWork = unitOfWork;
		_currentUser = currentUser;
	}


	public async Task<Result<GetMyTrackProgressResult>> Handle(GetMyTrackProgressQuery query,
															   CancellationToken ct = default)
	{
		if (!_currentUser.IsAuthenticated || _currentUser.UserId is not Guid userId)
			return Result<GetMyTrackProgressResult>.Failure(
				AuthErrors.UnAuthorized());

		User? user = await _unitOfWork.ReadRepository<User>()
									  .GetByIdAsync(id: userId,
													tracking: false,
													cancellationToken: ct);

		if (user is null || user.IsDeleted)
			return Result<GetMyTrackProgressResult>.Failure(
				AuthErrors.UnAuthorized());


		TrackProgress? trackProgress = await _unitOfWork.ReadRepository<TrackProgress>()
														.GetAsync(filter: tp => !tp.User.IsDeleted
																			 && !tp.Track.IsDeleted
																			 && !tp.Track.AudioFile.IsDeleted
																			 && !tp.Track.Course.IsDeleted
																			 && tp.UserId == userId
																			 && tp.TrackId == query.TrackId,
																  tracking: false,
																  cancellationToken: ct);
		if (trackProgress is null)
			return Result<GetMyTrackProgressResult>.Failure(
				TrackProgressErrors.NotFound());


		return Result<GetMyTrackProgressResult>.Success(
			new GetMyTrackProgressResult
			(
				Id: trackProgress.Id,
				TrackId: trackProgress.TrackId,
				PositionSec: trackProgress.PositionSec,
				Completed: trackProgress.Completed,
				UpdatedAt: trackProgress.UpdatedAt
			)
		);
	}

}
