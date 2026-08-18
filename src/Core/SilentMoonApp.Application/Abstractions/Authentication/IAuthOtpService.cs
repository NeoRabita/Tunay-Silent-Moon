using SilentMoonApp.Application.DTOs.Auth;


namespace SilentMoonApp.Application.Abstractions.Authentication;

public interface IAuthOtpService
{
	Task<GeneratedOtpResult> SendEmailConfirmationOtpAsync(User user,
														  string requestEmail,
														  CancellationToken cancellationToken = default);

	Task<GeneratedOtpResult> SendPasswordResetOtpAsync(User user,
													   string requestEmail,
													   CancellationToken cancellationToken = default);

	Task<Result> VerifyOtpAsync(Guid userId,
								EOtpPurpose otpPurpose,
								string rawCode,
								CancellationToken cancellationToken = default);
}
