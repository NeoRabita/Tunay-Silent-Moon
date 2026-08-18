using SilentMoonApp.Application.Errors;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Storage;


namespace SilentMoonApp.Application.Features.Profile.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandHandler : ICommandHandler<UpdateMyProfileCommand, UpdateMyProfileResult>
{
	private readonly ICurrentUser _currentUser;
	private readonly TimeProvider _timeProvider;
	private readonly IUserAvatarService _userAvatarService;
	private readonly IUnitOfWork _unitOfWork;

	public UpdateMyProfileCommandHandler(ICurrentUser currentUser,
										 TimeProvider timeProvider,
										 IUserAvatarService userAvatarService,
										 IUnitOfWork unitOfWork)
	{
		_currentUser = currentUser;
		_timeProvider = timeProvider;
		_userAvatarService = userAvatarService;
		_unitOfWork = unitOfWork;
	}



	public async Task<Result<UpdateMyProfileResult>> Handle(UpdateMyProfileCommand command,
															CancellationToken ct = default)
	{
		if (!_currentUser.IsAuthenticated ||
			 _currentUser.UserId is not Guid userId)

			return Result<UpdateMyProfileResult>.Failure(
				AuthErrors.UnAuthorized());


		User? user = await _unitOfWork.ReadRepository<User>().GetByIdAsync(id: userId,
																		   tracking: true,
																		   cancellationToken: ct);

		if (user is null || user.IsDeleted)
			return Result<UpdateMyProfileResult>.Failure(
				AuthErrors.UnAuthorized());


		DateTimeOffset nowUtc = _timeProvider.GetUtcNow();

		string? avatarUrl = null;


		if (command.AvatarFile is not null)
		{
			Result<string> uploadAvatarResult = await _userAvatarService.UploadAndAssignAvatarAsync(
				user: user,
				avatarFile: command.AvatarFile,
				actorUserId: userId,
				nowUtc: nowUtc,
				cancellationToken: ct);

			if (uploadAvatarResult.IsFailure)
				return Result<UpdateMyProfileResult>.Failure(uploadAvatarResult.Error);

			avatarUrl = uploadAvatarResult.Value;
		}

		else avatarUrl = await _userAvatarService.GetAvatarUrlAsync(user.AvatarImageFileId, ct);


		user.UpdatedAt = nowUtc;

		user.UpdatedBy = userId;


		return Result<UpdateMyProfileResult>.Success(
			new UpdateMyProfileResult
			(
				Id: userId,
				Name: user.FirstName,
				Email: user.Email,
				IsEmailVerified: user.IsEmailConfirmed,
				AvatarUrl: avatarUrl ?? string.Empty,
				CreatedAt: user.CreatedAt
			)
		);
	}
}
