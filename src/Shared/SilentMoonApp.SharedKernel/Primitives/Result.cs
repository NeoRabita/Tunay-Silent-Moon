namespace SilentMoonApp.SharedKernel.Primitives;

public class Result
{
	protected Result(bool isSuccess,
					 Error error)
	{
		ArgumentNullException.ThrowIfNull(error, nameof(error));


		if (isSuccess && error != Error.None)
			throw new InvalidOperationException("A successful result cannot contain an error.");

		if (!isSuccess && error == Error.None)
			throw new InvalidOperationException("A failed result must contain an error.");


		IsSuccess = isSuccess;
		Error = error;		
	}

	public bool IsSuccess { get; }
	public bool IsFailure => !IsSuccess;

	public Error Error { get; }


	public static Result Success()

		=> new Result(isSuccess: true,
					  error: Error.None);


	public static Result Failure(Error error)

		=> new Result(isSuccess: false,
					  error: error);
}



public class Result<TEntity> : Result
{
	private readonly TEntity? _entity;

	public Result(TEntity? entity,
				  bool isSuccess,
				  Error error)
		: base(isSuccess, error)
	{
		_entity = entity;
	}


	public TEntity Value
	{
		get
		{
			if (IsFailure)
				throw new InvalidOperationException("Cannot access the entity of a failed result.");

			return _entity!;
		}
	}


	public static Result<TEntity> Success(TEntity entity)

		=> new Result<TEntity>(entity: entity,
							   isSuccess: true,
							   error: Error.None);


	public static new Result<TEntity> Failure(Error error)

		=> new Result<TEntity>(entity: default,
							   isSuccess: false,
							   error: error);
}