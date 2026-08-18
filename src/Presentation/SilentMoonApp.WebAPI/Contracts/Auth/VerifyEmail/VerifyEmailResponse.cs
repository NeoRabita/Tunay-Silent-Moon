namespace SilentMoonApp.WebAPI.Contracts.Auth.VerifyEmail;

public sealed class VerifyEmailResponse
{
	public required string AccessToken { get; init; }
	public required string TokenType { get; init; }
	public required int AccessTokenExpiresIn { get; init; }

	public required VerifyEmailUserResponse User { get; init; }
}

public sealed class VerifyEmailUserResponse
{
	public required Guid Id { get; init; }
	public required string FirstName { get; init; }
	public required string Email { get; init; }
	public required bool IsEmailVerified { get; init; }
	public required string AvatarUrl { get; init; }
	public required DateTimeOffset CreatedAt { get; init; }
}
