using SilentMoonApp.Application.Helpers;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;


namespace SilentMoonApp.Application.Extensions;

public static class LoggingExtension
{
	public static TimedOperation BeginTimedOperation<TEntity>(this ILogger<TEntity> logger,
															  string operation,
															  LogLevel logLevel = LogLevel.Information,
															  [CallerMemberName] string method = "")
	{
		ArgumentNullException.ThrowIfNull(logger, nameof(logger));

		string component = GetComponentName(typeof(TEntity));


		return new TimedOperation
		(
			logger: logger,
			component: component,
			method: method,
			operation: operation,
			logLevel: logLevel
		);
	}


	private static string GetComponentName(Type type)
	{
		string name = type.Name;

		int genericMarkerIndex = name.IndexOf('`');
		

		return genericMarkerIndex > 0
			? name.Substring(0, genericMarkerIndex)
			: name;
	}
}
