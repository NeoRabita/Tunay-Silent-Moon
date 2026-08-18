using SilentMoonApp.Application.Abstractions.Communication.Email;

namespace SilentMoonApp.Application.Generators;

public static class EmailMessageGenerator
{
	public static EmailMessage GenerateVerificationEmail(string recipientEmail,
														 string otpCode,
														 int expirationMinutes)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(recipientEmail);
		ArgumentException.ThrowIfNullOrWhiteSpace(otpCode);


		if (expirationMinutes <= 0)
			throw new ArgumentOutOfRangeException(message: "OTP duration must be greater than 0.",
												  paramName: nameof(expirationMinutes),
												  actualValue: expirationMinutes);

		const string subject = "Verify your email address";
		string htmlBody = $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <title>Verify your email address</title>
            </head>
            <body>
                <h2>Verify your email address</h2>

                <p>
                    To Complete your registration,
                    please enter the code below:
                </p>

                <h1>{otpCode}</h1>

                <p>
                    The verification code {expirationMinutes}
                    is valid for {expirationMinutes} minutes.
                </p>
            </body>
            </html>
            """;

		string textBody = $"""
            Email Verification

            To complete your registration, please enter the code below:

            Your verification code: {otpCode}

            The Code is valid for {expirationMinutes} minutes.
            """;


		return new EmailMessage(to: recipientEmail,
								subject: subject,
								htmlBody: htmlBody,
								textBody: textBody);
	}


	public static EmailMessage GeneratePasswordResetEmail(string recipientEmail,
														  string otpCode,
														  long expirationMinutes)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(recipientEmail);
		ArgumentException.ThrowIfNullOrWhiteSpace(otpCode);


		if (expirationMinutes <= 0)
			throw new ArgumentOutOfRangeException(message: "OTP duration must be greater than 0.",
												  paramName: nameof(expirationMinutes),
												  actualValue: expirationMinutes);


		const string subject = "Reset Your Password";

		string htmlBody = $"""
			<!DOCTYPE html>
			<html lang="en">
			<head>
				<meta charset="UTF-8">
				<title>Reset Your Password</title>
			</head>
			<body>
				<h2>Reset Your Password</h2>

				<p>
					To Reset Your Password,
					please enter the code below:
				</p>

				<h1>{otpCode}</h1>

				<p>
					The Code {expirationMinutes} minutes
					is valid for {expirationMinutes} minutes.
				</p>

				<p>
					If you did not initiate this action,
					do not share the code with anyone and
					ignore this email.
				</p>
			</body>
			</html>
			""";

		string textBody = $"""
			Password Reset

			To reset your password, please enter the code below:

			Your password reset code: {otpCode}
			The code is valid for {expirationMinutes} minutes.

			If you did not initiate this action,
			do not share the code with anyone.
			""";


		return new EmailMessage(to: recipientEmail,
								subject: subject,
								htmlBody: htmlBody,
								textBody: textBody);
	}

}
