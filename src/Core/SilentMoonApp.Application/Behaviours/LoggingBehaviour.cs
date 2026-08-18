using Microsoft.Extensions.Logging;
using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Logging;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;
using SilentMoonApp.Application.Extensions;
using SilentMoonApp.Application.Helpers;
using System.Diagnostics;


namespace SilentMoonApp.Application.Behaviours;

public sealed class LoggingBehaviour<TRequest, TResponse>
	: IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
	private const int slowRequestThresholdMilliSeconds = 500;

	private readonly ICurrentUser _currentUser;
	private readonly IRequestContext _requestContext;
	private readonly ILogMasker _logMasker;
	private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;


	public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger,
							ICurrentUser currentUser,
							IRequestContext requestContext,
							ILogMasker logMasker)
	{
		_logger = logger;
		_currentUser = currentUser;
		_requestContext = requestContext;
		_logMasker = logMasker;
	}


	public async Task<Result<TResponse>> Handle(TRequest request,
												Func<Task<Result<TResponse>>> next,
												CancellationToken ct = default)
	{
		string requestName = typeof(TRequest).Name;

		string requestKind = request switch
		{
			ICommandBase => "Command",
			IQueryBase => "Query",
			_ => "Request"
		};


		string? traceId = _requestContext.TraceId
					   ?? Activity.Current?.TraceId.ToString();

		string? userId = _currentUser.IsAuthenticated
					   ? _currentUser.UserId.ToString()
					   : null;

		string? httpMethod = _requestContext.HttpMethod;

		string? path = _requestContext.Path;



		using IDisposable? logSpace = _logger.BeginScope(new Dictionary<string, object?>
		{
			["TraceId"] = traceId,
			["UserId"] = userId,

			["Path"] = path,
			["HttpMethod"] = httpMethod,

			["RequestName"] = requestName,
			["RequestKind"] = requestKind
		});



		using TimedOperation operation = _logger.BeginTimedOperation(operation: $"{requestKind}: {requestName}",
																	 logLevel: LogLevel.Information);

		_logger.LogDebug(message: "Handling {RequestKind} {RequestName} with TraceId: {TraceId} and UserId: {UserId}",
						 args: [requestKind, requestName, traceId, userId]);

		if (_logger.IsEnabled(LogLevel.Debug) && request is not INonLoggableCommand)
		{
			object? maskedRequest = _logMasker.Mask(request);

			_logger.LogDebug(message: "Request payload for {RequestKind} {RequestName}: {@Request}",
							 args: [requestKind, requestName, maskedRequest]);
		}


		Result<TResponse> response = await next();


		if (response is Result result &&
			result.IsFailure)
		{
			_logger.LogInformation(message: "Handled {RequestKind} {RequestName} with business failure in {ElapsedMilliseconds} ms. " +
											"ErrorCode: {ErrorCode}, ErrorType: {ErrorType}, TraceId: {TraceId}, UserId: {UserId}",
								   args: [requestKind, requestName, operation.ElapsedMilliseconds, result.Error.Code, result.Error.ErrorType, traceId, userId]);

			return response;
		}


		if (operation.ElapsedMilliseconds >= slowRequestThresholdMilliSeconds)
			_logger.LogWarning(message: "Handled Slowly {RequestKind} {RequestName} in {ElapsedMilliseconds} ms with TraceId: {TraceId} and UserId: {UserId}",
							   args: [requestKind, requestName, operation.ElapsedMilliseconds, traceId, userId]);

		else
			_logger.LogDebug(message: "Handled Successfully {RequestKind} {RequestName} in {ElapsedMilliseconds} ms with TraceId: {TraceId} and UserId: {UserId}",
							 args: [requestKind, requestName, operation.ElapsedMilliseconds, traceId, userId]);


		return response;
	}

}
