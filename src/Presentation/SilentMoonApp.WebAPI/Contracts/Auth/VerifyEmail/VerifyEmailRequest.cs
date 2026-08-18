namespace SilentMoonApp.WebAPI.Contracts.Auth.VerifyEmail;

public class VerifyEmailRequest
{
	public required string Email { get; init; }
	public required string OtpCode { get; init; }
}
