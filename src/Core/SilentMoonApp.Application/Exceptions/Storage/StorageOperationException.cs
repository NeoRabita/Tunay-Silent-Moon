using SilentMoonApp.Application.Exceptions.Common;

namespace SilentMoonApp.Application.Exceptions.Storage;

public class StorageOperationException : AppException
{
	public StorageOperationException(string message,
									 Exception? innerException = null) : base(code: "invalid.storage.operation",
																			  message: message,
																			  innerException)
	{ }
}
