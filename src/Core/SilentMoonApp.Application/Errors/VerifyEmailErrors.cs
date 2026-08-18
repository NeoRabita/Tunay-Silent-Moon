using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Errors;

public static class VerifyEmailErrors
{
	public static Error InvalidConfirmation()

		=> Error.Validation(code: "verify_email.invalid_confirmation",
								message: ErrorMessages.VerifyEmailInvalidConfirmation);


	public static Error AlreadyVerified()
		
		=> Error.Conflict(code: "verify_email.already_verified",
						  message: ErrorMessages.VerifyEmailAlreadyVerified);
	
}
