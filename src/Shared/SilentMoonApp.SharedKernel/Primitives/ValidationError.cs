using SilentMoonApp.SharedKernel.Resources;


namespace SilentMoonApp.SharedKernel.Primitives;

public record ValidationError : Error
{
	public ValidationError(IReadOnlyDictionary<string, string[]> errors) : base(code: "validation.failed",
																				message: ErrorMessages.ValidationFailed,
																				errorType: ErrorType.Validation,
																				details: errors)
	{
		ArgumentNullException.ThrowIfNull(errors, nameof(errors));

		if (errors.Count == 0)
			throw new ArgumentException("Validation errors cannot be empty.", nameof(errors));


		Errors = errors;
	}


	public IReadOnlyDictionary<string, string[]> Errors { get; }
}

