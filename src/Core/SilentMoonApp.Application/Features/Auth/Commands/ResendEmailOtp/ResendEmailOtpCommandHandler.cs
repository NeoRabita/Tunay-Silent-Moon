using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Domain.Enums;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Features.Auth.Commands.ResendEmailOtp;

public class ResendEmailOtpCommandHandler : ICommandHandler<ResendEmailOtpCommand, ResendEmailOtpResult>
{
	private static readonly string SuccessMessage = SuccessMessages.ResendEmailOtpSucceeded;

	private readonly IAuthOtpService _authOtpService;
	private readonly IUnitOfWork _unitOfWork;

	public ResendEmailOtpCommandHandler(IAuthOtpService authOtpService,
										IUnitOfWork unitOfWork)
	{
		_authOtpService = authOtpService;
		_unitOfWork = unitOfWork;
	}

	public async Task<Result<ResendEmailOtpResult>> Handle(ResendEmailOtpCommand command,
														   CancellationToken ct = default)
	{
		string normalizedEmail = command.Email.Trim().ToLowerInvariant();


		User? user = await _unitOfWork.Repository<IUserReadRepository>().GetByEmailAsync(normalizedEmail,
																						 tracking: false,
																						 cancellationToken: ct);

		if (user is null)
			return Result<ResendEmailOtpResult>.Failure(ResendEmailOtpErrors.InvalidRequest());

		if (user.IsEmailConfirmed)
			return Result<ResendEmailOtpResult>.Failure(VerifyEmailErrors.AlreadyVerified());

		if (user.UserStatus is not EUserStatus.PendingVerification)
			return Result<ResendEmailOtpResult>.Failure(ResendEmailOtpErrors.InvalidRequest());


		GeneratedOtpResult generatedOtp = await _authOtpService.SendEmailConfirmationOtpAsync(
			user: user,
			requestEmail: normalizedEmail,
			cancellationToken: ct);


		return Result<ResendEmailOtpResult>.Success(
			new ResendEmailOtpResult
			(
				Message: SuccessMessage,
				OtpExpiresAt: generatedOtp.ExpiresAt
			));
	}
}
