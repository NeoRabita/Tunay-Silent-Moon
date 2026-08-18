namespace SilentMoonApp.SharedKernel.Primitives;


public record Error
{
	protected Error(string code,
					string message,
					ErrorType errorType,
					object? details = null)
	{
		Code = code;
		Message = message;
		ErrorType = errorType;
		Details = details;
	}


	public string Code { get; }
	public string Message { get; }
	public ErrorType ErrorType { get; }
	public object? Details { get; }



	public static readonly Error None = new(code: string.Empty,
											message: string.Empty,
											errorType: ErrorType.None);


	public static Error Validation(string code,
								   string message,
								   object? details = null) => new(code: code,
																 message: message,
																 errorType: ErrorType.Validation,
																 details: details);

	public static Error NotFound(string code,
								 string message,
								 object? details = null) => new(code: code,
																message: message,
																errorType: ErrorType.NotFound,
																details: details);

	public static Error Conflict(string code,
								 string message,
								 object? details = null) => new(code: code,
																message: message,
																errorType: ErrorType.Conflict,
																details: details);

	public static Error UnAuthorized(string code,
									 string message,
									 object? details = null) => new(code: code,
																	message: message,
																	errorType: ErrorType.UnAuthorized,
																	details: details);

	public static Error Forbidden(string code,
								  string message,
								  object? details = null) => new(code: code,
																 message: message,
																 errorType: ErrorType.Forbidden,
																 details: details);

	public static Error BusinessRule(string code,
									 string message,
									 object? details = null) => new(code: code,
																	message: message,
																	errorType: ErrorType.BusinessRule,
																	details: details);

	public static Error UnAvailable(string code,
								    string message,
								    object? details = null) => new(code: code,
																   message: message,
																   errorType: ErrorType.UnAvailable,
																   details: details);

	public static Error TooManyRequests(string code,
										string message,
										object? details = null) => new(code: code,
																	   message: message,
																	   errorType: ErrorType.TooManyRequests,
																	   details: details);

	public static Error Locked(string code,
						   string message,
						   object? details = null) => new(code: code,
														  message: message,
														  errorType: ErrorType.Locked,
														  details: details);
}
