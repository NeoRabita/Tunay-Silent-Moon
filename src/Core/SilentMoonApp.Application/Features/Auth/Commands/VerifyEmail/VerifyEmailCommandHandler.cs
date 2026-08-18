using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Storage;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Domain.Enums;


namespace SilentMoonApp.Application.Features.Auth.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : ICommandHandler<VerifyEmailCommand, VerifyEmailResult>
{
	private readonly TimeProvider _timeProvider;
	private readonly IAuthTokenService _authTokenService;
	private readonly IAuthOtpService _authOtpService;
	private readonly IUserAvatarService _userAvatarService;
	private readonly IUnitOfWork _unitOfWork;

	public VerifyEmailCommandHandler(TimeProvider timeProvider,
									 IAuthTokenService authTokenService,
									 IAuthOtpService authOtpService,
									 IUserAvatarService userAvatarService,
									 IUnitOfWork unitOfWork)
	{
		_timeProvider = timeProvider;
		_authTokenService = authTokenService;
		_authOtpService = authOtpService;
		_userAvatarService = userAvatarService;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<VerifyEmailResult>> Handle(VerifyEmailCommand command,
														CancellationToken ct = default)
	{
		string normalizedEmail = command.Email.Trim().ToLowerInvariant();
		
		string rawOtpCode = command.OtpCode.Trim();


		User? user = await _unitOfWork.Repository<IUserReadRepository>().GetByEmailAsync(
			normalizedEmail,
			tracking: true,
			cancellationToken: ct);

		
		if (user is null)
			return Result<VerifyEmailResult>.Failure(OtpErrors.InvalidCode());

		if (user.IsEmailConfirmed)
			return Result<VerifyEmailResult>.Failure(VerifyEmailErrors.AlreadyVerified());

		if (user.UserStatus is not EUserStatus.PendingVerification)
			return Result<VerifyEmailResult>.Failure(VerifyEmailErrors.InvalidConfirmation());


		Result otpVerificationResult = await _authOtpService.VerifyOtpAsync(
			userId: user.Id,
			otpPurpose: EOtpPurpose.EmailConfirmation,
			rawCode: rawOtpCode,
			cancellationToken: ct);


		if (otpVerificationResult.IsFailure)
			return Result<VerifyEmailResult>.Failure(otpVerificationResult.Error);

		DateTimeOffset nowUtc = _timeProvider.GetUtcNow();

		user.IsEmailConfirmed = true;
		user.ConfirmedAt = nowUtc;
		
		user.UserStatus = EUserStatus.Active;
		user.UpdatedAt = nowUtc;
		
		user.AccessFailedCount = 0;
		user.LockoutEndAt = null;


		var session = await _authTokenService.GenerateSessionAsync(user, ct);

		string avatarUrl = await _userAvatarService.GetAvatarUrlAsync(user.AvatarImageFileId, ct);


		return Result<VerifyEmailResult>.Success(
			new VerifyEmailResult
			(
				AccessToken: session.AccessToken,
				RefreshToken: session.RefreshToken,
				TokenType: session.TokenType,
				RefreshTokenExpiresAt: session.RefreshTokenExpiresAt,
				AccessTokenExpiresIn: session.AccessTokenExpiresIn,
				User: new VerifyEmailUserResult
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
