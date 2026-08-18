namespace SilentMoonApp.Application.Features.Profile.Commands.UpdateMyProfile;

public sealed record UpdateMyProfileResult
(
	Guid Id,
	string Name,
	string Email,
	bool IsEmailVerified,
	string AvatarUrl,
	DateTimeOffset CreatedAt
);
