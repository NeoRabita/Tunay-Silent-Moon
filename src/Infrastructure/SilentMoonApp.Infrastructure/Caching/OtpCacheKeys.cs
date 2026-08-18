using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.Infrastructure.Caching;

internal static class OtpCacheKeys
{
	private const string Prefix = "project:v1:otp";

	public static string GenerateKey(Guid userId,
									 EOtpPurpose otpPurpose)
	{
		if (userId == Guid.Empty)
			throw new ArgumentException(message: "UserId cannot be empty.",
										paramName: nameof(userId));


		string purposeSegment = otpPurpose switch
		{
			EOtpPurpose.EmailConfirmation => "email-confirmation",

			EOtpPurpose.PasswordReset => "password-reset",

			EOtpPurpose.PhoneVerification => "phone-verification",

			EOtpPurpose.TwoFactorAuthentication => "two-factor",

			_ => throw new ArgumentOutOfRangeException(paramName: nameof(otpPurpose),
													   message: $"Unsupported OTP Purpose: {otpPurpose}.")
		};


		return $"{Prefix}:{purposeSegment}:{userId:N}";
	}

}
