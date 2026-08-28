namespace SilentMoonApp.WebAPI.Contracts.Common;

public class PaginationResponse<TEntity>
{
	public IReadOnlyList<TEntity> Data { get; init; } = new List<TEntity>();
	public required PaginationResponseMeta Meta { get; init; }
}


public class PaginationResponseMeta
{
	public int PageNumber { get; init; }
	public int PageSize { get; init; }
	public int TotalCount { get; init; }
	public int TotalPages { get; init; }
}

