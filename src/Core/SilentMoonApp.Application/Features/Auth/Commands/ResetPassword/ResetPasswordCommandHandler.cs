using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Hashing;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandHandler : ICommandHandler<ResetPasswordCommand, ResetPasswordResult>
{
	private readonly TimeProvider _timeProvider;
	private readonly IAuthOtpService _authOtpService;
	private readonly IPasswordHasher _passwordHasher;
	private readonly IAuthTokenService _authTokenService;
	private readonly IUnitOfWork _unitOfWork;

	public ResetPasswordCommandHandler(TimeProvider timeProvider,
									   IAuthOtpService authOtpService,
									   IPasswordHasher passwordHasher,
									   IAuthTokenService authTokenService,
									   IUnitOfWork unitOfWork)
	{
		_timeProvider = timeProvider;
		_authOtpService = authOtpService;
		_passwordHasher = passwordHasher;
		_authTokenService = authTokenService;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<ResetPasswordResult>> Handle(ResetPasswordCommand command,
														  CancellationToken ct = default)
	{
		string normalizedEmail = command.Email.Trim().ToLowerInvariant();
		string rawOtpCode = command.OtpCode.Trim();

		User? user = await _unitOfWork.Repository<IUserReadRepository>().GetByEmailAsync(
			email: normalizedEmail,
			tracking: true,
			cancellationToken: ct);

		if (user is null ||
			user.IsDeleted ||
			!user.IsEmailConfirmed ||
			user.UserStatus is not EUserStatus.Active)
		{
			return Result<ResetPasswordResult>.Failure(ResetPasswordErrors.InvalidRequest());
		}

		Result otpVerificationResult = await _authOtpService.VerifyOtpAsync(
			userId: user.Id,
			otpPurpose: EOtpPurpose.PasswordReset,
			rawCode: rawOtpCode,
			cancellationToken: ct);

		if (otpVerificationResult.IsFailure)
			return Result<ResetPasswordResult>.Failure(otpVerificationResult.Error);

		if (!string.IsNullOrWhiteSpace(user.PasswordHash) &&
			_passwordHasher.Verify(command.NewPassword, user.PasswordHash))
		{
			return Result<ResetPasswordResult>.Failure(ResetPasswordErrors.SameAsCurrentPassword());
		}

		DateTimeOffset nowUtc = _timeProvider.GetUtcNow();

		user.PasswordHash = _passwordHasher.Hash(command.NewPassword);
		user.AccessFailedCount = 0;
		user.LockoutEndAt = null;
		user.UpdatedAt = nowUtc;

		await _authTokenService.RevokeActiveRefreshTokensAsync(
			userId: user.Id,
			revocationReason: ERevocationReason.PasswordChanged,
			nowUtc: nowUtc,
			cancellationToken: ct);

		return Result<ResetPasswordResult>.Success(
			new ResetPasswordResult
			(
				Message: SuccessMessages.ResetPasswordSucceeded
			));
	}
}
