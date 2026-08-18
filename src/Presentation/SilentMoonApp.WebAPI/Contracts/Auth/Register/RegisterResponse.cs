namespace SilentMoonApp.WebAPI.Contracts.Auth.Register;

public sealed class RegisterResponse
{
	public required string Message { get; init; }
	public required string Email { get; init; }
	public required DateTimeOffset OtpExpiresAt { get; init; }
}
