using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Domain.Enums;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, ForgotPasswordResult>
{
	private readonly TimeProvider _timeProvider;
	private readonly IAuthOtpService _authOtpService;
	private readonly IUnitOfWork _unitOfWork;

	public ForgotPasswordCommandHandler(TimeProvider timeProvider,
										IAuthOtpService authOtpService,
										IUnitOfWork unitOfWork)
	{
		_timeProvider = timeProvider;
		_authOtpService = authOtpService;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<ForgotPasswordResult>> Handle(ForgotPasswordCommand command,
														   CancellationToken ct = default)
	{
		string normalizedEmail = command.Email.Trim().ToLowerInvariant();

		User? user = await _unitOfWork.Repository<IUserReadRepository>().GetByEmailAsync(
			email: normalizedEmail,
			tracking: false,
			cancellationToken: ct);

		if (user is null ||
			user.IsDeleted ||
			!user.IsEmailConfirmed ||
			user.UserStatus is not EUserStatus.Active)
		{
			return Result<ForgotPasswordResult>.Success(
				new ForgotPasswordResult
				(
					Message: SuccessMessages.ForgotPasswordCodeSentIfEmailExists,
					Email: normalizedEmail,
					OtpExpiresAt: _timeProvider.GetUtcNow().AddMinutes(5)
				));
		}

		GeneratedOtpResult generatedOtp = await _authOtpService.SendPasswordResetOtpAsync(
			user: user,
			requestEmail: normalizedEmail,
			cancellationToken: ct);

		return Result<ForgotPasswordResult>.Success(
			new ForgotPasswordResult
			(
				Message: SuccessMessages.ForgotPasswordRequestAccepted,
				Email: normalizedEmail,
				OtpExpiresAt: generatedOtp.ExpiresAt
			));
	}
}
