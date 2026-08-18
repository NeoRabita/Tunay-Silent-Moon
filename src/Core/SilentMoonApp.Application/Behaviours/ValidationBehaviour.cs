using FluentValidation;
using FluentValidation.Results;
using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;


namespace SilentMoonApp.Application.Behaviours;

public sealed class ValidationBehaviour<TRequest, TResponse> 
	: IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
	private readonly IValidator<TRequest>[] _validators;


	public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
	{
		_validators = validators.ToArray();
	}



	public async Task<Result<TResponse>> Handle(TRequest request, Func<Task<Result<TResponse>>> next,
										CancellationToken ct = default)
	{
		if (_validators.Length is 0)
			return await next();


		ValidationContext<TRequest> context = new(request);


		ValidationResult[] results = await Task.WhenAll(_validators
											   .Select(validator => validator.ValidateAsync(context, ct)));


		//foreach (IValidator<TRequest> validator in _validators)
		//{
		//	ValidationResult result =
		//		await validator.ValidateAsync(context, ct);

		//	if (!result.IsValid)
		//		failures.AddRange(result.Errors);
		//}


		//ValidationFailure[] failures = results.SelectMany(result => result.Errors)
		//									  .Where(failure => failure is not null)
		//									  .ToArray();


		ValidationFailure[] failures = results.SelectMany(result => result.Errors)
											  .ToArray();

		if(failures.Length is 0)
			return await next();


		IReadOnlyDictionary<string, string[]> errors = failures.GroupBy(failure => failure.PropertyName)
															   .ToDictionary(group => group.Key,
																			 group => group.Select(failure => failure.ErrorMessage)
																						   .Distinct()
																						   .ToArray());

		return Result<TResponse>.Failure(
			new ValidationError(errors));
	}

}
