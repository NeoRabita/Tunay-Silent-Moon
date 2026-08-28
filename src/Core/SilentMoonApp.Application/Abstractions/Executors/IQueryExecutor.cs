using System.Linq.Expressions;


namespace SilentMoonApp.Application.Abstractions.Executors;

public interface IQueryExecutor
{
	Task<int> CountAsync<TEntity>(IQueryable<TEntity> query,
								  CancellationToken cancellationToken = default);

	Task<List<TEntity>> ToListAsync<TEntity>(IQueryable<TEntity> query,
													CancellationToken cancellationToken = default);

	Task<List<Result>> ToListAsync<TEntity, Result>(IQueryable<TEntity> query,
														   Expression<Func<TEntity, Result>> filter,
														   CancellationToken cancellationToken = default);
}
