using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Errors;

public static class ResetPasswordErrors
{
	public static Error InvalidRequest()

		=> Error.Validation(code: "reset_password.invalid_request",
							message: ErrorMessages.ResetPasswordInvalidRequest);


	public static Error SameAsCurrentPassword()

		=> Error.Validation(code: "reset_password.same_as_current_password",
							message: ErrorMessages.ResetPasswordSameAsCurrentPassword);

}
