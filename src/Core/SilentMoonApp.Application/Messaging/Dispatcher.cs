using Microsoft.Extensions.DependencyInjection;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;


namespace SilentMoonApp.Application.Messaging;

public class Dispatcher : IDispatcher
{
	private readonly IServiceProvider _serviceProvider;

	public Dispatcher(IServiceProvider serviceProvider)
	{
		_serviceProvider = serviceProvider;
	}



	public async Task<Result<NoResult>> SendAsync(ICommand command,
												  CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(command);

		return await DispatchAsync<NoResult>(command, ct);
	}

	public async Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command,
															  CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(command);

		return await DispatchAsync(command, ct);
	}


	public async Task<Result<TResponse>> SendAsync<TResponse>(IQuery<TResponse> query,
														      CancellationToken ct = default)
	{
		ArgumentNullException.ThrowIfNull(query);

		return await DispatchAsync(query, ct);
	}




	// Helpers

	private async Task<Result<TResponse>> DispatchAsync<TResponse>(IRequest<TResponse> request,
																   CancellationToken ct = default)
	{
		Type requestType = request.GetType();

		Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType,
																	  typeof(TResponse));


		object handler = _serviceProvider.GetService(handlerType)
					  ?? throw new InvalidOperationException($"Handler for request type {requestType.Name} not found.");


		Type behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));

		object[] behaviors = _serviceProvider.GetServices(behaviorType)
											 .Where(static behavior => behavior is not null)
											 .Cast<object>()
											 .Reverse()
											 .ToArray();


		Func<Task<Result<TResponse>>> next = ()
			  => ((dynamic)handler).Handle((dynamic)request, ct);


		foreach (var behavior in behaviors)
		{
			Func<Task<Result<TResponse>>> currentNext = next;

			next = () => ((dynamic)behavior).Handle((dynamic)request,
													currentNext,
													ct);
		}


		return await next();
	}

}

