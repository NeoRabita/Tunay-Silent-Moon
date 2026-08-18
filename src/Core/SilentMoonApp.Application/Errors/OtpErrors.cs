using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.SharedKernel.Resources;

namespace SilentMoonApp.Application.Errors;

public static class OtpErrors
{
	public static Error InvalidCode()

		 => Error.Validation(code: "otp.invalid_code",
							 message: ErrorMessages.OtpInvalidCode);


	public static Error AttemptsExceeded(int remainingAttemps)

		 => Error.TooManyRequests(code: "otp.attempts_exceeded",
								  message: ErrorMessages.OtpAttemptsExceeded,
								  details: new
								  {
									  remainingAttemps
								  });


	public static Error Unavailable()

		 => Error.Validation(code: "otp.unavailable",
							 message: ErrorMessages.OtpUnavailable);

}
