namespace SilentMoonApp.Application.Features.Auth.Commands.Register;

public sealed record RegisterResult
(
	string Message,
	string Email,
	DateTimeOffset OtpExpiresAt
);
