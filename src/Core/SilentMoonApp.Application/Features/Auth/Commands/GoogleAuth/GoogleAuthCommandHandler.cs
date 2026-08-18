using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Domain.Enums;
using SilentMoonApp.SharedKernel.Primitives;

namespace SilentMoonApp.Application.Features.Auth.Commands.GoogleAuth;

public class GoogleAuthCommandHandler : ICommandHandler<GoogleAuthCommand, GoogleAuthResult>
{
	private readonly IAuthTokenService _authTokenService;
	private readonly IUserAvatarService _userAvatarService;
	private readonly IExternalAuthUserService _externalAuthUserService;
	private readonly IReadOnlyCollection<IExternalAuthProvider> _externalAuthProviders;

	public GoogleAuthCommandHandler(IAuthTokenService authTokenService,
									IUserAvatarService userAvatarService,
									IExternalAuthUserService externalAuthUserService,
									IEnumerable<IExternalAuthProvider> externalAuthProviders)
	{
		_authTokenService = authTokenService;
		_userAvatarService = userAvatarService;
		_externalAuthUserService = externalAuthUserService;
		_externalAuthProviders = externalAuthProviders.ToArray();
	}

	public async Task<Result<GoogleAuthResult>> Handle(GoogleAuthCommand command,
													   CancellationToken ct = default)
	{
		IExternalAuthProvider googleProvider = _externalAuthProviders.SingleOrDefault(
			provider => provider.Provider is EExternalAuthProvider.Google)
			?? throw new InvalidOperationException("Google provider not found.");


		Result<ExternalAuthProviderResult> verificationResult = await googleProvider.VerifyAsync(
			providerToken: command.IdToken,
			cancellationToken: ct);


		if (verificationResult.IsFailure)
			return Result<GoogleAuthResult>.Failure(verificationResult.Error);


		ExternalAuthProviderResult providerResult = verificationResult.Value;

		if (providerResult.Provider is not EExternalAuthProvider.Google)
			throw new InvalidOperationException(
				$"Invalid provider result. Expected: {EExternalAuthProvider.Google}, Actual: {providerResult.Provider}");

		
		Result<User> userResult = await _externalAuthUserService.GetOrGenerateActiveUserAsync(providerResult, ct);

		if (userResult.IsFailure)
			return Result<GoogleAuthResult>.Failure(userResult.Error);

		
		User user = userResult.Value;
		
		var session = await _authTokenService.GenerateSessionAsync(user, ct);
		
		string avatarUrl = await _userAvatarService.GetAvatarUrlAsync(user.AvatarImageFileId, ct);


		return Result<GoogleAuthResult>.Success(
			new GoogleAuthResult
			(
				AccessToken: session.AccessToken,
				RefreshToken: session.RefreshToken,
				TokenType: session.TokenType,
				AccessTokenExpiresIn: session.AccessTokenExpiresIn,
				RefreshTokenExpiresAt: session.RefreshTokenExpiresAt,
				User: new GoogleAuthUserResult
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
