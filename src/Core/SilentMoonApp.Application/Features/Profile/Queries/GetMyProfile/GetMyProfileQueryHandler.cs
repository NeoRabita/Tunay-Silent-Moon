using SilentMoonApp.Application.Errors;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Storage;


namespace SilentMoonApp.Application.Features.Profile.Queries.GetMyProfile;

public class GetMyProfileQueryHandler : IQueryHandler<GetMyProfileQuery, GetMyProfileResult>
{
	private readonly ICurrentUser _currentUser;
	private readonly IUnitOfWork _unitOfWork;
	private readonly IUserAvatarService _userAvatarService;

	public GetMyProfileQueryHandler(ICurrentUser currentUser,
									IUnitOfWork unitOfWork,
									IUserAvatarService userAvatarService)
	{
		_currentUser = currentUser;
		_unitOfWork = unitOfWork;
		_userAvatarService = userAvatarService;
	}
	public async Task<Result<GetMyProfileResult>> Handle(GetMyProfileQuery request, CancellationToken ct = default)
	{
		if (!_currentUser.IsAuthenticated)
			return Result<GetMyProfileResult>.Failure(
				AuthErrors.UnAuthorized());


		Guid? userId = _currentUser.UserId;


		if (userId is null)
			return Result<GetMyProfileResult>.Failure(
				AuthErrors.UnAuthorized());


		User? user = await _unitOfWork.ReadRepository<User>().GetByIdAsync(id: userId.Value,
																		   tracking: false,
																		   cancellationToken: ct);

		if (user is null || user.IsDeleted)
			return Result<GetMyProfileResult>.Failure(
				AuthErrors.UnAuthorized());


		string avatarUrl = await _userAvatarService.GetAvatarUrlAsync(user.AvatarImageFileId, ct);


		return Result<GetMyProfileResult>.Success(
			new GetMyProfileResult
			(
				Id: user.Id,
				Name: user.FirstName,
				Email: user.Email,
				IsEmailVerified: user.IsEmailConfirmed,
				AvatarUrl: avatarUrl,
				CreatedAt: user.CreatedAt
			));
	}
}
