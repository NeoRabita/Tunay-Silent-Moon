using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Domain.Enums;
using SilentMoonApp.SharedKernel.Primitives;

namespace SilentMoonApp.Application.Features.Auth.Commands.FacebookAuth;

public class FacebookAuthCommandHandler : ICommandHandler<FacebookAuthCommand, FacebookAuthResult>
{
	private readonly IAuthTokenService _authTokenService;
	private readonly IUserAvatarService _userAvatarService;
	private readonly IExternalAuthUserService _externalAuthUserService;
	private readonly IReadOnlyCollection<IExternalAuthProvider> _externalAuthProviders;

	public FacebookAuthCommandHandler(IAuthTokenService authTokenService,
									  IUserAvatarService userAvatarService,
									  IExternalAuthUserService externalAuthUserService,
									  IEnumerable<IExternalAuthProvider> externalAuthProviders)
	{
		_authTokenService = authTokenService;
		_userAvatarService = userAvatarService;
		_externalAuthUserService = externalAuthUserService;
		_externalAuthProviders = externalAuthProviders.ToArray();
	}

	public async Task<Result<FacebookAuthResult>> Handle(FacebookAuthCommand command,
														 CancellationToken ct = default)
	{
		IExternalAuthProvider facebookProvider = _externalAuthProviders.SingleOrDefault(
			provider => provider.Provider is EExternalAuthProvider.Facebook)
			?? throw new InvalidOperationException("Facebook provider not found.");


		Result<ExternalAuthProviderResult> verificationResult = await facebookProvider.VerifyAsync(
			providerToken: command.IdToken,
			cancellationToken: ct);


		if (verificationResult.IsFailure)
			return Result<FacebookAuthResult>.Failure(verificationResult.Error);


		ExternalAuthProviderResult providerResult = verificationResult.Value;


		if (providerResult.Provider is not EExternalAuthProvider.Facebook)
			throw new InvalidOperationException(
				$"Invalid provider result. Expected: {EExternalAuthProvider.Facebook}, Actual: {providerResult.Provider}");


		Result<User> userResult = await _externalAuthUserService.GetOrGenerateActiveUserAsync(providerResult, ct);


		if (userResult.IsFailure)
			return Result<FacebookAuthResult>.Failure(userResult.Error);


		User user = userResult.Value;
		
		var session = await _authTokenService.GenerateSessionAsync(user, ct);
		
		string avatarUrl = await _userAvatarService.GetAvatarUrlAsync(user.AvatarImageFileId, ct);


		return Result<FacebookAuthResult>.Success(
			new FacebookAuthResult
			(
				AccessToken: session.AccessToken,
				RefreshToken: session.RefreshToken,
				TokenType: session.TokenType,
				AccessTokenExpiresIn: session.AccessTokenExpiresIn,
				RefreshTokenExpiresAt: session.RefreshTokenExpiresAt,
				User: new FacebookAuthUserResult
				(
					Id: user.Id,
					Name: user.FirstName,
					Email: user.Email,
					EmailVerified: user.IsEmailConfirmed,
					AvatarUrl: avatarUrl,
					CreatedAt: user.CreatedAt
				)
			));
	}
}
