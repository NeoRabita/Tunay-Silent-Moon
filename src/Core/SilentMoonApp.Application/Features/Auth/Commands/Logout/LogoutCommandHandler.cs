using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.Messaging;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Domain.Enums;


namespace SilentMoonApp.Application.Features.Auth.Commands.Logout;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand, NoResult>
{
	private readonly TimeProvider _timeProvider;
	private readonly IAuthTokenService _authTokenService;

	public LogoutCommandHandler(TimeProvider timeProvider,
								IAuthTokenService authTokenService)
	{
		_timeProvider = timeProvider;
		_authTokenService = authTokenService;
	}


	public async Task<Result<NoResult>> Handle(LogoutCommand command,
											   CancellationToken ct = default)
	{
		if (string.IsNullOrWhiteSpace(command.RefreshToken))
			return Result<NoResult>.Failure(AuthErrors.InvalidRefreshToken());

		
		Result<RefreshToken> refreshTokenResult = await _authTokenService.GetActiveRefreshTokenAsync(
			rawRefreshToken: command.RefreshToken,
			cancellationToken: ct);


		if (refreshTokenResult.IsFailure)
			//return Result<NoResult>.Success(new NoResult());
			return Result<NoResult>.Failure(AuthErrors.InvalidRefreshToken());


		RefreshToken refreshToken = refreshTokenResult.Value;
		
		refreshToken.RevokedAt = _timeProvider.GetUtcNow();
		refreshToken.RevocationReason = ERevocationReason.UserLogout;

		return Result<NoResult>.Success(new NoResult());
	}
}
