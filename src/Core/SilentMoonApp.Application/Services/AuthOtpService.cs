using Microsoft.Extensions.Logging;
using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Caching;
using SilentMoonApp.Application.Abstractions.Communication.Email;
using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.Extensions;
using SilentMoonApp.Application.Generators;
using SilentMoonApp.Application.Helpers;


namespace SilentMoonApp.Application.Services;

public sealed class AuthOtpService : IAuthOtpService
{
	private readonly IOtpService _otpService;
	private readonly TimeProvider _timeProvider;
	private readonly IEmailService _emailService;
	private readonly IOtpCacheService _otpCacheService;
	private readonly ILogger<AuthOtpService> _logger;

	public AuthOtpService(IOtpService otpService,
						  TimeProvider timeProvider,
						  IEmailService emailService,
						  IOtpCacheService otpCacheService,
						  ILogger<AuthOtpService> logger)
	{
		_otpService = otpService;
		_timeProvider = timeProvider;
		_emailService = emailService;
		_otpCacheService = otpCacheService;
		_logger = logger;
	}



	public async Task<GeneratedOtpResult> SendEmailConfirmationOtpAsync(User user,
																		string requestEmail,
																		CancellationToken cancellationToken = default)
	{
		using TimedOperation operation = _logger.BeginTimedOperation(operation: "SendEmailConfirmationOTP",
																	 logLevel: LogLevel.Information);


		GeneratedOtpResult generatedOtp = await GenerateAndStoreOtpAsync(userId: user.Id,
																		 otpPurpose: EOtpPurpose.EmailConfirmation,
																		 cancellationToken: cancellationToken);

		EmailMessage emailMessage = EmailMessageGenerator.GenerateVerificationEmail(otpCode: generatedOtp.RawCode,
																					recipientEmail: user.Email,
																					expirationMinutes: GetExpirationMinutes(generatedOtp));

		await SendEmailAsync(emailMessage, requestEmail, user.Email, cancellationToken);

		return generatedOtp;
	}


	public async Task<GeneratedOtpResult> SendPasswordResetOtpAsync(User user,
																	string requestEmail,
																	CancellationToken cancellationToken = default)
	{
		using TimedOperation operation = _logger.BeginTimedOperation(operation: "SendPasswordResetOTP",
																	 logLevel: LogLevel.Information);


		GeneratedOtpResult generatedOtp = await GenerateAndStoreOtpAsync(userId: user.Id,
																		 otpPurpose: EOtpPurpose.PasswordReset,
																		 cancellationToken: cancellationToken);

		EmailMessage emailMessage = EmailMessageGenerator.GeneratePasswordResetEmail(otpCode: generatedOtp.RawCode,
																					 recipientEmail: user.Email,
																					 expirationMinutes: GetExpirationMinutes(generatedOtp));

		await SendEmailAsync(emailMessage, requestEmail, user.Email, cancellationToken);


		return generatedOtp;
	}


	public async Task<Result> VerifyOtpAsync(Guid userId,
											 EOtpPurpose otpPurpose,
											 string rawCode,
											 CancellationToken cancellationToken = default)
	{
		using TimedOperation operation = _logger.BeginTimedOperation(operation: "VerifyOTP",
																	 logLevel: LogLevel.Information);


		OtpCacheVerificationResult otpVerificationResult = await _otpCacheService.VerifyOtpAsync(
			userId: userId,
			otpPurpose: otpPurpose,
			rawCode: rawCode,
			ct: cancellationToken);


		return otpVerificationResult.OtpVerificationStatus switch
		{
			EOtpVerificationStatus.Succeeded => Result.Success(),

			EOtpVerificationStatus.InvalidCode => Result.Failure(
				OtpErrors.InvalidCode()),

			EOtpVerificationStatus.Unavailable => Result.Failure(
				OtpErrors.Unavailable()),

			EOtpVerificationStatus.AttemptsExceeded => Result.Failure(
				OtpErrors.AttemptsExceeded(otpVerificationResult.RemainingAttempts)),


			_ => throw new InvalidOperationException($"Unexpected OTP verification status: {otpVerificationResult.OtpVerificationStatus}")
		};
	}



	// Helpers

	private async Task<GeneratedOtpResult> GenerateAndStoreOtpAsync(Guid userId,
																   EOtpPurpose otpPurpose,
																   CancellationToken cancellationToken)
	{
		GeneratedOtpResult generatedOtp = _otpService.GenerateVerificationCode();

		await _otpCacheService.StoreOtpAsync(userId: userId,
											 otpPurpose: otpPurpose,
											 rawCode: generatedOtp.RawCode,
											 expiresAt: generatedOtp.ExpiresAt,
											 ct: cancellationToken);

		return generatedOtp;
	}


	private async Task SendEmailAsync(EmailMessage emailMessage,
									  string requestEmail,
									  string userEmail,
									  CancellationToken cancellationToken)
	{
		//_logger.LogInformation(message: "Email göndəriləcək. RequestEmail: [{RequestEmail}], UserEmail: [{UserEmail}]",
		//					   args: [requestEmail, userEmail]);

		await _emailService.SendAsync(
			emailMessage: emailMessage,
			cancellationToken: cancellationToken);
	}


	private int GetExpirationMinutes(GeneratedOtpResult generatedOtp)
		=> Convert.ToInt32((generatedOtp.ExpiresAt - _timeProvider.GetUtcNow()).TotalMinutes);
}
