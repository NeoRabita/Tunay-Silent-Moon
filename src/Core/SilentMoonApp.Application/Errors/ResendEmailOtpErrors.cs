using SilentMoonApp.SharedKernel.Resources;
using SilentMoonApp.SharedKernel.Primitives;


namespace SilentMoonApp.Application.Errors;

public static class ResendEmailOtpErrors
{
	public static Error InvalidRequest()

		=> Error.Validation(code: "resend_email_otp.invalid_request",
							message: ErrorMessages.ResendEmailOtpInvalidRequest);

}
