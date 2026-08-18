using Microsoft.Extensions.Logging;
using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;
using SilentMoonApp.Application.Abstractions.Repositories;
using System.Diagnostics;


namespace SilentMoonApp.Application.Behaviours;

public sealed class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
											   where TRequest : IRequest<TResponse>
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;

	public TransactionBehaviour(IUnitOfWork unitOfWork,
								ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
	{
		_unitOfWork = unitOfWork;
		_logger = logger;
	}



	public async Task<Result<TResponse>> Handle(TRequest request,
								  Func<Task<Result<TResponse>>> next,
								  CancellationToken ct = default)
	{
		if (request is not ICommandBase)
			return await next();

		if (_unitOfWork.HasActiveTransaction)
			return await next();

		if (request is INonTransactionalCommand)
		{
			Result<TResponse> nonTransactionalResponse = await next();

			await _unitOfWork.SaveChangesAsync(ct);


			return nonTransactionalResponse;
		}


		string requestName = typeof(TRequest).Name;


		await _unitOfWork.BeginTransactionAsync(ct);

	

		try
		{
			Result<TResponse> response = await next();


			if (response is Result result &&
				result.IsFailure)
			{
				await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);

				_logger.LogWarning("----- Rollback Database Transaction for {CommandName}, due to failure ErrorCode:{ErrorCode}",
								   requestName, result.Error.Code);

				return response;
			}

			await _unitOfWork.SaveChangesAsync(ct);

			await _unitOfWork.CommitTransactionAsync(ct);

			_logger.LogDebug(message: "----- Commit Database Transaction for {CommandName}",
							 args: requestName);

			return response;
		}

		catch
		{
			await _unitOfWork.RollbackTransactionAsync(CancellationToken.None);

			_logger.LogWarning(message: "----- Rollback Database Transaction for {CommandName}",
							   args: requestName);

			throw;
		}

	}

}
