using System.Linq.Expressions;

namespace SilentMoonApp.Application.Extensions;

public static class QuerySortExtensions
{
	public static IQueryable<TEntity> ApplySort<TEntity, TSortBy>(this IQueryable<TEntity> query,
																  TSortBy? sortBy,
																  TSortBy defaultSortBy,
																  IReadOnlyDictionary<TSortBy, Expression<Func<TEntity, object>>> sortFilter,
																  ESortDirection? sortDirection = ESortDirection.Descending) where TSortBy : struct, Enum
	{
		TSortBy selectedSortBy = sortBy ?? defaultSortBy;

		if (!sortFilter.TryGetValue(selectedSortBy, out Expression<Func<TEntity, object>>? sortExpression))
			sortExpression = sortFilter[defaultSortBy];


		return sortDirection == ESortDirection.Ascending
							  ? query.OrderBy(sortExpression)
							  : query.OrderByDescending(sortExpression);
	}
}
