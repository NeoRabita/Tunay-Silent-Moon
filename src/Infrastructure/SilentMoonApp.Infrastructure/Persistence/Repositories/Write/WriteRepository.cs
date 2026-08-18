using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Application.Abstractions.Repositories.Write;
using SilentMoonApp.Domain.Entities.Common;
using SilentMoonApp.Infrastructure.Persistence.Contexts;
using System.Threading.Tasks;

namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Write;

public class WriteRepository<TEntity> : IWriteRepository<TEntity> where TEntity : BaseEntity
{

	private readonly AppDbContext _dbContext;
	protected DbSet<TEntity> Table => _dbContext.Set<TEntity>();


	public WriteRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}


	public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
			=> await Table.AddAsync(entity, cancellationToken);

	public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
			=> await Table.AddRangeAsync(entities, cancellationToken);

	public void Update(TEntity entity)
			=> Table.Update(entity);

	public void Remove(TEntity entity)
			=> Table.Remove(entity);

	public void RemoveRange(IEnumerable<TEntity> entities)
			=> Table.RemoveRange(entities);

}
