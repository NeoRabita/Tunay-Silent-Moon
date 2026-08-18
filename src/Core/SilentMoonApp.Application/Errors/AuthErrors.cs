using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Errors;

public static class AuthErrors
{
	public static Error UnAuthorized()

		=> Error.UnAuthorized(code: "auth.unauthorized",
							  message: ErrorMessages.AuthUnAuthorized);


	public static Error InvalidCredentials()

		=> Error.UnAuthorized(code: "auth.invalid_credentials",
							  message: ErrorMessages.AuthInvalidCredentials);


	public static Error AccountBlocked(DateTimeOffset lockOutEndAt)

		=> Error.Locked(code: "auth.account_locked",
						message: ErrorMessages.AuthAccountLocked,
						details: new
						{
							LockOutEndAt = lockOutEndAt
						});


	public static Error EmailNotVerified()

		=> Error.Forbidden(code: "auth.email_not_verified",
						   message: ErrorMessages.AuthEmailNotVerified);


	public static Error InvalidRefreshToken()

		=> Error.NotFound(code: "auth.invalid_refresh_token",
						  message: ErrorMessages.AuthInvalidRefreshToken);
}
