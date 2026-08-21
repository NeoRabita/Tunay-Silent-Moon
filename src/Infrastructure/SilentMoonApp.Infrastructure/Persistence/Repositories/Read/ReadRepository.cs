using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Domain.Entities.Common;
using SilentMoonApp.Infrastructure.Persistence.Contexts;
using SilentMoonApp.Application.Abstractions.Repositories.Read;


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


	public async Task<IReadOnlyList<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null,
													 Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null,
													 bool tracking = false, CancellationToken ct = default)
			=> await Query(filter, includes, tracking).ToListAsync(ct);

	public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>>? filter = null,
								   Func<IQueryable<TEntity>, IQueryable<TEntity>>? includes = null,
								   bool tracking = false, CancellationToken ct = default)
			=> await Query(filter, includes, tracking).FirstOrDefaultAsync(ct);


	public async Task<TEntity?> GetByIdAsync(Guid id, bool tracking = false,
									   CancellationToken ct = default)
			=> await (tracking ? Table : Table.AsNoTracking()).FirstOrDefaultAsync(e => e.Id == id, ct);


	public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>>? filter = null,
							   CancellationToken ct = default)
			=> filter is null
				  ? await Table.AnyAsync(ct)
				  : await Table.AnyAsync(filter, ct);

	public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null,
								CancellationToken ct = default)
			=> filter is null
				  ? await Table.CountAsync(ct)
				  : await Table.CountAsync(filter, ct);
}
