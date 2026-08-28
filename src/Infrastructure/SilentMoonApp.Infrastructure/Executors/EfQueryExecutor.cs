using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Application.Abstractions.Executors;


namespace SilentMoonApp.Infrastructure.Executors;

public class EfQueryExecutor : IQueryExecutor
{
	public Task<int> CountAsync<TEntity>(IQueryable<TEntity> query,
										 CancellationToken ct = default)
		=> query.CountAsync(cancellationToken: ct);


	public Task<List<TEntity>> ToListAsync<TEntity>(IQueryable<TEntity> query,
												    CancellationToken ct = default)
		=> query.ToListAsync(cancellationToken: ct);

	public Task<List<Result>> ToListAsync<TEntity, Result>(IQueryable<TEntity> query,
														   Expression<Func<TEntity, Result>> filter,
														   CancellationToken ct = default)
		=> query.Select(filter)
				.ToListAsync(cancellationToken: ct);
}
