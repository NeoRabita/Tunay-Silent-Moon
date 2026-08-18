namespace SilentMoonApp.WebAPI.Contracts.Profile.UpdateMyProfile;

public sealed class UpdateMyProfileRequest
{
	public required string Name { get; set; } = null!;
	public IFormFile? AvatarImage { get; set; }
}
