namespace SilentMoonApp.Application.Features.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordResult
(
	string Message,
	string Email,
	DateTimeOffset OtpExpiresAt
);