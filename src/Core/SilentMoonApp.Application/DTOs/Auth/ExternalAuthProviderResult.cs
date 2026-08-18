using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.Application.DTOs.Auth;

public sealed record ExternalAuthProviderResult
(
	EExternalAuthProvider Provider,
	string ProviderUserId,
	string? Email,
	string? FirstName,
	string? LastName,
	string? AvatarUrl
);
