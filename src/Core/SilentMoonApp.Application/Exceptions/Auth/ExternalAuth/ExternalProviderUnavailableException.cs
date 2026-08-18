using SilentMoonApp.Application.Exceptions.Common;
using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.Application.Exceptions.Auth.ExternalAuth;

public class ExternalProviderUnavailableException : AppException
{
	public ExternalProviderUnavailableException(EExternalAuthProvider provider,
												Exception? innerException = null) : base(code: "external_provider_unavailable",
																						 message:$"{provider} authentication service is currently unavailable.",
																					     innerException: innerException)
	{ }
}
