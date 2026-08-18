using SilentMoonApp.Application.Exceptions.Common;

namespace SilentMoonApp.Application.Exceptions.Email;

public class SmtpTransientException : AppException
{
	public SmtpTransientException(string code,
								   string message,
								   Exception? innerException = null) : base(code: code,
																			message: message,
																			innerException: innerException)
	{ }
}
