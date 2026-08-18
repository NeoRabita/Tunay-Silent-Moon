namespace SilentMoonApp.WebAPI.Contracts.Auth.ResendEmailOtp;

public sealed class ResendEmailOtpResponse
{
	public required string Message { get; init; }
	public DateTimeOffset OtpExpiresAt { get; init; }
}
