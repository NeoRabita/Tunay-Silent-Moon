namespace SilentMoonApp.WebAPI.Contracts.Auth.ResetPassword;

public sealed class ResetPasswordRequest
{
	public required string Email { get; set; }
	public required string Otp { get; set; }
	public required string NewPassword { get; set; }
	public required string ConfirmPassword { get; set; }
}
