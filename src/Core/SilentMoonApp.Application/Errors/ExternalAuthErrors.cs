using SilentMoonApp.Domain.Enums;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Errors;

public static class ExternalAuthErrors
{
	public static Error EmailRequired()

		=> Error.Validation(code: "external_auth.email_required",
							message: ErrorMessages.ExternalAuthEmailRequired);


	public static Error AccountLinkRequired()

		=> Error.Conflict(code: "external_auth.account_link_required",
						  message: ErrorMessages.ExternalAuthAccountLinkRequired);


	public static Error AccountUnavailable()

		=> Error.Forbidden(code: "external_auth.account_unavailable",
						   message: ErrorMessages.ExternalAuthAccountUnavailable);


	public static Error InvalidProviderToken(EExternalAuthProvider externalProvider)
		
		=> Error.UnAuthorized(code: "external_auth.invalid_provider_token",
							  message: ErrorMessages.ExternalAuthInvalidProviderToken,
							  details: new
							  {
							  	  provider = externalProvider,
							  });
}
