namespace SilentMoonApp.WebAPI.Contracts.Auth.ForgotPassword;

public sealed class ForgotPasswordResponse
{
	public required string Message { get; set; }
	public required string Email { get; set; }
	public required DateTimeOffset OtpExpiresAt { get; set; }
}
