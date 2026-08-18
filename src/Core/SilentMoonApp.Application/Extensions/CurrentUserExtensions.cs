using SilentMoonApp.Application.Abstractions.Authentication;

namespace SilentMoonApp.Application.Extensions;

public static class CurrentUserExtensions
{
	public static Guid GetRequiredUserId(this ICurrentUser currentUser)
	{
		ArgumentNullException.ThrowIfNull(currentUser);

		if (!currentUser.IsAuthenticated || currentUser.UserId is null)
			throw new UnauthorizedAccessException("User is not authenticated.");


		return currentUser.UserId
			?? throw new UnauthorizedAccessException("The Authenticated request does not contain a Valid User Identifier.");
	}

	public static bool IsInRole(this ICurrentUser currentUser, string role)
	{
		ArgumentNullException.ThrowIfNull(currentUser);

		if (string.IsNullOrEmpty(role))
			return false;

		if (currentUser.Roles is null || currentUser.Roles.Count == 0)
			return false;

		return currentUser.Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
	}

}
