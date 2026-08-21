using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;


namespace SilentMoonApp.Application.Helpers;

public class TimedOperation : IDisposable
{
	private readonly ILogger _logger;
	private readonly string _component;
	private readonly string _method;
	private readonly string _operation;
	private readonly LogLevel _logLevel;

	private readonly long _startedAt;
	private bool _disposed;


	internal TimedOperation(ILogger logger,
							string component,
							string method,
							string operation,
							LogLevel logLevel)
	{
		_logger = logger;
		_component = component;
		_method = method;
		_operation = operation;
		_logLevel = logLevel;

		_startedAt = Stopwatch.GetTimestamp();
	}


	public double ElapsedMilliseconds => Stopwatch.GetElapsedTime(_startedAt)
												  .TotalMilliseconds;

	public static int GenerateDaysMask(IEnumerable<EWeekDay> weekDays)
	{
		int mask = 0;

		foreach (var day in weekDays)
			mask |= 1 << ((int)day - 1);

		return mask;
	}


	public static IReadOnlyList<EWeekDay> DecodeDaysMask(int mask)

		=> Enum.GetValues<EWeekDay>()
				.Where(day => (mask & (1 << ((int)day - 1))) != 0)
				.ToArray();


	public void Dispose()
	{
		if (_disposed)
			return;

		double durationMs = ElapsedMilliseconds;

		_logger.Log(logLevel: _logLevel,

					message: "Operation timing recorded. " +
							 "Component: {Component}, " +
							 "Method: {Method}, " +
							 "Operation: {Operation}, " +
							 "DurationMs: {DurationMs:F2}",

					args: [_component, _method, _operation, durationMs]);


		_disposed = true;
	}

}

