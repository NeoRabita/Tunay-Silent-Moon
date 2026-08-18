using SilentMoonApp.SharedKernel.Primitives;


namespace SilentMoonApp.Application.Abstractions.Messaging;

public interface IRequestHandler<in TRequest, TResponse>
	where TRequest : IRequest<TResponse>
{
	Task<Result<TResponse>> Handle(TRequest request,
						   CancellationToken cancellationToken = default);
}
