using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Diagnostics;
using SilentMoonApp.Application.Exceptions.Common;
using SilentMoonApp.Application.Exceptions.Auth.ExternalAuth;


namespace SilentMoonApp.WebAPI.Middlewares;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
	private readonly ILogger<GlobalExceptionHandler> _logger;
	private readonly IWebHostEnvironment _environment;


	public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger,
								  IWebHostEnvironment environment)
	{
		_logger = logger;
		_environment = environment;
	}



	public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
												Exception exception,
												CancellationToken ct)
	{
		string traceId = Activity.Current?.TraceId.ToString()
					  ?? httpContext.TraceIdentifier;

		ProblemDetails problemDetails = GenerateProblemDetails(httpContext,
															   exception,
															   traceId);

		LogException(httpContext: httpContext,
					 exception: exception,
					 statusCode: problemDetails.Status,
					 traceId: traceId);


		httpContext.Response.StatusCode = problemDetails.Status
									   ?? StatusCodes.Status500InternalServerError;

		httpContext.Response.ContentType = "application/problem+json";


		await httpContext.Response.WriteAsJsonAsync(value: problemDetails,
													cancellationToken: ct);

		return true;
	}


	private ProblemDetails GenerateProblemDetails(HttpContext httpContext,
												  Exception exception,
												  string traceId)
	{
		ProblemDetails problemDetails = exception switch
		{
			ExternalProviderUnavailableException =>
				new ProblemDetails
				{
					Type = "urn:problem-type:external-provider-unavailable",

					Status = StatusCodes.Status503ServiceUnavailable,

					Title = "External Provider Unavailable",

					Detail = _environment.IsDevelopment()
						   ? exception.Message
						   : "External provider is unavailable.",
				},


			_ => GenerateInternalServerProblem(exception)
		};


		problemDetails.Instance = httpContext.Request.Path;

		problemDetails.Extensions["traceId"] = traceId;

		problemDetails.Extensions["code"] = exception is AppException appException
										  ? appException.Code
										  : "server.internal_error";

		return problemDetails;
	}


	private ProblemDetails GenerateInternalServerProblem(Exception exception)

		=> new ProblemDetails
		{
			Type = "urn:problem-type:server.internal_error",

			Status = StatusCodes.Status500InternalServerError,

			Title = "Internal server error",

			Detail = _environment.IsDevelopment()
				   ? exception.Message
				   : "An unexpected error occurred. Please contact support if the problem persists."
		};


	private void LogException(HttpContext httpContext,
							  Exception exception,
							  int? statusCode,
							  string traceId)
	{
		string path = httpContext.Request.Path.Value
				   ?? "/";


		_logger.LogError(exception: exception,
						 message: "Handled exception occurred. " +
						          "ExceptionType: {ExceptionType}, " +
								  "StatusCode: {StatusCode}, " +
						          "Path: {Path}, " +
								  "TraceId: {TraceId}",

						 args: [exception.GetType().Name,
						        statusCode,
								path,
								traceId]);
	}

}