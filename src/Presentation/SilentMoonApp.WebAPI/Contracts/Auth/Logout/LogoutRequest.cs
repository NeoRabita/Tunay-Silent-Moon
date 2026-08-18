namespace SilentMoonApp.WebAPI.Contracts.Auth.Logout;

public sealed class LogoutRequest
{
	public string? RefreshToken { get; init; }
}
