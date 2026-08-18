using SilentMoonApp.Application.Exceptions.Common;

namespace SilentMoonApp.Application.Exceptions.Email;

public class SmtpPermanentException : AppException
{
	public SmtpPermanentException(string code,
								   string message,
								   Exception? innerException = null) : base(code: code,
																			message: message,
																			innerException: innerException)
	{ }
}
