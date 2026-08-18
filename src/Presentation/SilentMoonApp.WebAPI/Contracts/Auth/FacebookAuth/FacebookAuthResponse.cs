namespace SilentMoonApp.WebAPI.Contracts.Auth.FacebookAuth;

public sealed class FacebookAuthResponse
{
	public required string AccessToken { get; init; }
	public required string TokenType { get; init; }
	public required int AccessTokenExpiresIn { get; init; }

	public required FacebookAuthUserResponse User { get; init; }
}


public sealed class FacebookAuthUserResponse
{
	public required Guid Id { get; init; }
	public required string FirstName { get; init; }
	public required string Email { get; init; }
	public required bool IsEmailVerified { get; init; }
	public required string AvatarUrl { get; init; }
	public required DateTimeOffset CreatedAt { get; init; }
}