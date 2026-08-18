using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SilentMoonApp.SharedKernel.Primitives;


namespace SilentMoonApp.WebAPI.Controllers;

[ApiController]
public abstract class BaseController : ControllerBase
{
	protected IActionResult HandleResult<TEntity>(Result<TEntity> result)
												
		=> result.IsFailure
			? HandleError(result.Error)
			: Ok(result.Value);


	protected IActionResult HandleResult<TEntity>(Result<TEntity> result,
												  Func<TEntity, IActionResult> onSuccess)
		=> result.IsFailure
			? HandleError(result.Error)
			: onSuccess(result.Value);



	// Helpers

	private IActionResult HandleError(Error error)
	{
		string traceId = Activity.Current?.TraceId.ToString()
					  ?? HttpContext.TraceIdentifier;

		if (error is ValidationError validationError)
		{
			ValidationProblemDetails validationProblemDetails = new(validationError.Errors.ToDictionary(item => item.Key,
																										item => item.Value))
			{
				Type = $"urn:problem-type:{validationError.Code}",

				Status = StatusCodes.Status422UnprocessableEntity,

				Title = "Request Validation Failed",

				Detail = validationError.Message,

				Instance = HttpContext.Request.Path
			};


			validationProblemDetails.Extensions["code"] = validationError.Code;

			validationProblemDetails.Extensions["traceId"] = traceId;


			return new ObjectResult(validationProblemDetails)
			{
				StatusCode = StatusCodes.Status422UnprocessableEntity
			};
		}


		(int statusCode, string title) = error.ErrorType switch
		{
			ErrorType.Validation =>
			(
				statusCode = StatusCodes.Status422UnprocessableEntity,
				title = "Validation Error"
			),

			ErrorType.NotFound =>
			(
				statusCode = StatusCodes.Status404NotFound,
				title = "Resource Not Found"
			),

			ErrorType.Conflict =>
			(
				statusCode = StatusCodes.Status409Conflict,
				title = "Resource Conflict"
			),

			ErrorType.UnAuthorized =>
			(
				statusCode = StatusCodes.Status401Unauthorized,
				title = "Unauthorized Access"
			),

			ErrorType.Forbidden =>
			(
				statusCode = StatusCodes.Status403Forbidden,
				title = "Forbidden Access"
			),

			ErrorType.BusinessRule =>
			(
				statusCode = StatusCodes.Status400BadRequest,
				title = "Business Rule Violation"
			),

			ErrorType.UnAvailable =>
			(
				statusCode = StatusCodes.Status503ServiceUnavailable,
				title = "Service Unavailable"
			),

			ErrorType.ExternalProvider =>
			(
				statusCode = StatusCodes.Status502BadGateway,
				title = "External Provider Error"
			),

			ErrorType.TooManyRequests =>
			(
				statusCode = StatusCodes.Status429TooManyRequests,
				title = "Too Many Requests"
			),

			ErrorType.Locked =>
			(
				statusCode = StatusCodes.Status423Locked,
				title = "Account Locked"
			),	


			_ => throw new InvalidOperationException($"Unhandled error type: {error.ErrorType}")
		};


		ProblemDetails problemDetails = new()
		{
			Type = $"urn:problem-type:{error.Code}",
			Status = statusCode,
			Title = title,
			Detail = error.Message,
			Instance = HttpContext.Request.Path
		};


		problemDetails.Extensions["code"] = error.Code;

		problemDetails.Extensions["traceId"] = traceId;


		return new ObjectResult(problemDetails)
		{
			StatusCode = statusCode
		};
	}
}
