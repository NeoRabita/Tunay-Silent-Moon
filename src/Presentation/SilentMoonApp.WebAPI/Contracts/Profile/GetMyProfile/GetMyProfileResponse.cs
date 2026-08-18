namespace SilentMoonApp.WebAPI.Contracts.Profile.GetMyProfile;

public sealed class GetMyProfileResponse
{
	public required Guid Id { get; init; }
	public required string Name { get; init; }
	public required string Email { get; init; }
	public required bool IsEmailVerified { get; init; }
	public string AvatarUrl { get; init; } = string.Empty;
	public required DateTimeOffset CreatedAt { get; init; }
}
