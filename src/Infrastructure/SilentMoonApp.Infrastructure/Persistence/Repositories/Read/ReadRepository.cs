using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Domain.Entities.Common;
using SilentMoonApp.Infrastructure.Persistence.Contexts;
using System.Linq.Expressions;

namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Read;

public class ReadRepository<TEntity> : IReadRepository<TEntity> where TEntity : BaseEntity
{
	private readonly AppDbContext _dbContext;
	public DbSet<TEntity> Table => _dbContext.Set<TEntity>();


	public ReadRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}


	public IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? filter = null,
									 Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null,
									 bool tracking = false)
	{
		var query = Table.AsQueryable();

		if (filter is not null)
			query = query.Where(filter);

		if (includes is not null)
			query = includes(query);

		if (!tracking)
			query = query.AsNoTracking();

		return query;
	}


	public Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? filter = null,
								   Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null,
								   bool tracking = false, CancellationToken ct = default)
			=> Query(filter, includes, tracking).FirstOrDefaultAsync(ct);


	public Task<TEntity?> GetByIdAsync(Guid id, bool tracking = false,
									   CancellationToken ct = default)
			=> (tracking ? Table : Table.AsNoTracking()).FirstOrDefaultAsync(e => e.Id == id, ct);


	public Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? filter = null,
							   CancellationToken ct = default)
			=> filter is null
				  ? Table.AnyAsync(ct)
				  : Table.AnyAsync(filter, ct);


	public Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null,
								CancellationToken ct = default)
			=> filter is null
				  ? Table.CountAsync(ct)
				  : Table.CountAsync(filter, ct);
}
