namespace SilentMoonApp.WebAPI.Contracts.Auth.ForgotPassword;

public sealed class ForgotPasswordRequest
{
	public required string Email { get; set; }
}
