namespace SilentMoonApp.Application.Features.Profile.Queries.GetMyProfile;

public sealed record GetMyProfileResult
(
	Guid Id,
	string Name,
	string Email,
	bool IsEmailVerified,
	string AvatarUrl,
	DateTimeOffset CreatedAt
);
