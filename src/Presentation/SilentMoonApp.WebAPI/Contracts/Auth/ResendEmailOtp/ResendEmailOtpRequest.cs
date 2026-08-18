namespace SilentMoonApp.WebAPI.Contracts.Auth.ResendEmailOtp;

public sealed class ResendEmailOtpRequest
{
	public required string Email { get; init; }
}
