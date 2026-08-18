namespace SilentMoonApp.Application.Abstractions.Caching;

public interface ICacheService
{
	Task<TEntity?> GetAsync<TEntity>(string key,
									 CancellationToken cancellationToken = default);

	Task SetAsync<TEntity>(string key,
						   TEntity entity,
						   TimeSpan? expiration = null,
						   CancellationToken cancellationToken = default);

	Task<bool> RemoveAsync(string key,
					 CancellationToken cancellationToken = default);

	Task<bool> ExistsAsync(string key,
						   CancellationToken cancellationToken = default);
}
