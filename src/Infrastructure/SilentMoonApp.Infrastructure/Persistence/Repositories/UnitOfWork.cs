using Microsoft.EntityFrameworkCore.Storage;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Repositories.Write;
using SilentMoonApp.Infrastructure.Persistence.Contexts;


namespace SilentMoonApp.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
	private readonly AppDbContext _dbContext;
	private IDbContextTransaction? _transaction;

	private readonly IServiceProvider _serviceProvider;
	private readonly Dictionary<Type, object> _repositories = [];


	public UnitOfWork(AppDbContext dbContext,
					  IServiceProvider serviceProvider)
	{
		_dbContext = dbContext;
		_serviceProvider = serviceProvider;
	}



	public bool HasActiveTransaction
		=> _transaction is not null;


	public IReadRepository<TEntity> ReadRepository<TEntity>() where TEntity : class
		=> Repository<IReadRepository<TEntity>>();

	public IWriteRepository<TEntity> WriteRepository<TEntity>() where TEntity : class
		=> Repository<IWriteRepository<TEntity>>();


	public TRepository Repository<TRepository>() where TRepository : notnull
	{
		Type type = typeof(TRepository);

		if (_repositories.TryGetValue(type, out var repository))
			return (TRepository)repository;

		TRepository resolvedRepository = _serviceProvider.GetRequiredService<TRepository>();

		_repositories[type] = resolvedRepository;


		return resolvedRepository;
	}


	public Task<int> SaveChangesAsync(CancellationToken ct = default)
		=> _dbContext.SaveChangesAsync(ct);


	public async Task BeginTransactionAsync(CancellationToken ct = default)
	{
		if (_transaction is not null)
			throw new InvalidOperationException("There is already an active database transaction.");


		_transaction = await _dbContext.Database.BeginTransactionAsync(ct);
	}


	public async Task CommitTransactionAsync(CancellationToken ct = default)
	{
		if (_transaction is null)
			throw new InvalidOperationException("There is no active transaction to commit.");


		try
		{
			await _transaction!.CommitAsync(ct);
		}
		finally
		{
			await DisposeTransactionAsync();

			_transaction = null;
		}
	}


	public async Task RollbackTransactionAsync(CancellationToken ct = default)
	{
		if (_transaction is null)
			return;


		try
		{
			await _transaction!.RollbackAsync(ct);
		}
		finally
		{
			_transaction = null;

			await DisposeTransactionAsync();

			_dbContext.ChangeTracker.Clear();
		}
	}



	// Helpers

	private async ValueTask DisposeTransactionAsync()
	{
		if (_transaction is null)
			return;


		IDbContextTransaction transaction = _transaction;

		_transaction = null;

		await transaction.DisposeAsync();
	}
}
