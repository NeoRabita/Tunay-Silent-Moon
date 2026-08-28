namespace SilentMoonApp.Application.DTOs.Common;

public class PaginationResult<TEntity>
{
	public required PaginationResultMeta Meta { get; init; } 
	public IReadOnlyList<TEntity> Data { get; init; } = new List<TEntity>();
}


public class  PaginationResultMeta
{
	public int PageNumber { get; init; }
	public int PageSize { get; init; }
	public int TotalCount { get; init; }
	public int TotalPages { get; init; }
}
