using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Errors;

public static class UserErrors
{
	public static Error EmailAlreadyExsists(string? customMessage = null)
		
		=> Error.Conflict(code: "user.email_already_exists",
						  message: ErrorMessages.EmailAlreadyExists);


	public static Error UserNameAlreadyExists(string? customMessage = null)

		=> Error.Conflict(code: "user.username_already_exists",
						  message: ErrorMessages.UserNameAlreadyExists);

}
