using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.SharedKernel.Primitives;

namespace SilentMoonApp.Application.Features.Auth.Commands.Refresh;

public class RefreshCommandHandler : ICommandHandler<RefreshCommand, RefreshResult>
{
	private readonly TimeProvider _timeProvider;
	private readonly IAuthTokenService _authTokenService;
	private readonly IUserAvatarService _userAvatarService;

	public RefreshCommandHandler(TimeProvider timeProvider,
								 IAuthTokenService authTokenService,
								 IUserAvatarService userAvatarService)
	{
		_timeProvider = timeProvider;
		_authTokenService = authTokenService;
		_userAvatarService = userAvatarService;
	}


	public async Task<Result<RefreshResult>> Handle(RefreshCommand command,
													CancellationToken ct = default)
	{
		Result<RefreshToken> currentRefreshTokenResult = await _authTokenService.GetActiveRefreshTokenAsync(rawRefreshToken: command.RefreshToken,
																											cancellationToken: ct);

		if (currentRefreshTokenResult.IsFailure)
			return Result<RefreshResult>.Failure(currentRefreshTokenResult.Error);

		RefreshToken currentRefreshToken = currentRefreshTokenResult.Value;

		User user = currentRefreshToken.User;


		if (user.IsDeleted || !user.IsEmailConfirmed)
			return Result<RefreshResult>.Failure(AuthErrors.InvalidRefreshToken());


		DateTimeOffset nowUtc = _timeProvider.GetUtcNow();
		
		var session = await _authTokenService.GenerateSessionAsync(user, ct);

		currentRefreshToken.UsedAt = nowUtc;
		currentRefreshToken.ReplacedTokenId = session.RefreshTokenId;

		string avatarUrl = await _userAvatarService.GetAvatarUrlAsync(user.AvatarImageFileId, ct);


		return Result<RefreshResult>.Success(
			new RefreshResult
			(
				AccessToken: session.AccessToken,
				RefreshToken: session.RefreshToken,
				TokenType: session.TokenType,
				RefreshTokenExpiresAt: session.RefreshTokenExpiresAt,
				AccessTokenExpiresIn: session.AccessTokenExpiresIn,
				User: new RefreshUserResult
				(
					Id: user.Id,
					FirstName: user.FirstName,
					Email: user.Email,
					IsEmailVerified: user.IsEmailConfirmed,
					AvatarUrl: avatarUrl,
					CreatedAt: user.CreatedAt
				)
			));
	}
}
