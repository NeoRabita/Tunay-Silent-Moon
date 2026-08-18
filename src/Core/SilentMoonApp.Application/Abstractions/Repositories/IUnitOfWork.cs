using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Repositories.Write;


namespace SilentMoonApp.Application.Abstractions.Repositories;

public interface IUnitOfWork
{
	bool HasActiveTransaction {  get; }

	IReadRepository<TEntity> ReadRepository<TEntity>() where TEntity : class;
	IWriteRepository<TEntity> WriteRepository<TEntity>() where TEntity : class;
	TRepository Repository<TRepository>() where TRepository : notnull;

	Task BeginTransactionAsync(CancellationToken cancellationToken = default);
	Task CommitTransactionAsync(CancellationToken cancellationToken = default);
	Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
