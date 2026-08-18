using SilentMoonApp.SharedKernel.Primitives;


namespace SilentMoonApp.Application.Abstractions.Messaging.Execution;

public interface IPipelineBehavior<in TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	Task<Result<TResponse>> Handle(TRequest request,
								   Func<Task<Result<TResponse>>> next,
								   CancellationToken cancellationToken = default);
}
