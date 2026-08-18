namespace SilentMoonApp.WebAPI.Contracts.Auth.GoogleAuth;

public sealed class GoogleAuthRequest
{
	public required string idToken { get; init; }
}
