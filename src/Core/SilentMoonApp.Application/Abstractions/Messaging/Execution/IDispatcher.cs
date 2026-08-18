using SilentMoonApp.Application.Messaging;
using SilentMoonApp.SharedKernel.Primitives;

namespace SilentMoonApp.Application.Abstractions.Messaging.Execution;

public interface IDispatcher
{
	Task<Result<NoResult>> SendAsync(ICommand command,
									 CancellationToken cancellationToken = default);

	Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command,
												 CancellationToken cancellationToken = default);

	Task<Result<TResponse>> SendAsync<TResponse>(IQuery<TResponse> query,
												 CancellationToken cancellationToken = default);
}
