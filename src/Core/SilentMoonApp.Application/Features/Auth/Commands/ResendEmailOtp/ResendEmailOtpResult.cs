namespace SilentMoonApp.Application.Features.Auth.Commands.ResendEmailOtp;

public sealed record ResendEmailOtpResult
(
	string Message,
	DateTimeOffset OtpExpiresAt
);