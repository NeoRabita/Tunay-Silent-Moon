using SilentMoonApp.Application.Abstractions.Executors;
using SilentMoonApp.Application.DTOs.Common;

namespace SilentMoonApp.Application.Extensions;

public static class QueryPaginationExtensions
{
	public static async Task<PaginationResult<TEntity>> PaginateAsync<TEntity>(this IQueryable<TEntity> query,
																			   IQueryExecutor queryExecutor,
																			   PaginationQueryRequest paginationRequest,
																			   CancellationToken cancellationToken = default) where TEntity : class
	{
		int pageNumber = paginationRequest.PageNumber <= 0
					   ? 1
					   : paginationRequest.PageNumber;

		int pageSize = paginationRequest.PageSize <= 0
					 ? 20
					 : paginationRequest.PageSize;

		int totalCount = await queryExecutor.CountAsync(query, cancellationToken);

		List<TEntity> data = await queryExecutor.ToListAsync(query: query.Skip((pageNumber - 1) * pageSize)
																		.Take(pageSize),
															 cancellationToken: cancellationToken);

		return new PaginationResult<TEntity>
		{
			Data = data,
			Meta = new PaginationResultMeta
			{
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalCount = totalCount,
				TotalPages = totalCount == 0
										 ? 0
										 : (int)Math.Ceiling(totalCount / (double)pageSize)
			}
		};
	}
}
