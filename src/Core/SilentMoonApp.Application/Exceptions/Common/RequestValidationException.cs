using FluentValidation.Results;

namespace SilentMoonApp.Application.Exceptions.Common;

public sealed class RequestValidationException : AppException
{
	public RequestValidationException(IEnumerable<ValidationFailure> failures)
		: base(code: "validation.failed",
			   message: "One or more validation errors occurred.")
	{
		Errors = failures.GroupBy(failure => failure.PropertyName)
						 .ToDictionary(group => group.Key,
									   group => group.Select(failure => failure.ErrorMessage)
													 .Distinct()
													 .ToArray());
	}

	public IReadOnlyDictionary<string, string[]> Errors { get; }
}
