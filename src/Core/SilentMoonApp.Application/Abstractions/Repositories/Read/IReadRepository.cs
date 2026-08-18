using System.Linq.Expressions;

namespace SilentMoonApp.Application.Abstractions.Repositories.Read;

public interface IReadRepository<TEntity> where TEntity : class
{
	IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? filter = null,
							  Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null,
							  bool tracking = false);

	Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? filter = null,
							Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null,
							bool tracking = false,
							CancellationToken cancellationToken = default);

	Task<TEntity?> GetByIdAsync(Guid id, bool tracking = false,
								CancellationToken cancellationToken = default);

	Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? filter = null,
						CancellationToken cancellationToken = default);

	Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null,
						 CancellationToken cancellationToken = default);
}
