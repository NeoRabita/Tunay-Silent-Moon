using System.Text.Json;
using StackExchange.Redis;
using SilentMoonApp.Application.Abstractions.Caching;


namespace SilentMoonApp.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
	private readonly IDatabase _database;

	private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);


	public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
	{
		ArgumentNullException.ThrowIfNull(connectionMultiplexer);

		_database = connectionMultiplexer.GetDatabase();
	}



	public async Task<TEntity?> GetAsync<TEntity>(string key,
												  CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(key);

		RedisValue cachedValue = await _database.StringGetAsync(key)
												.WaitAsync(ct);

		if (cachedValue.IsNull)
			return default;

		return JsonSerializer.Deserialize<TEntity>(json: cachedValue.ToString(),
												   options: _jsonOptions);
	}


	public async Task SetAsync<TEntity>(string key,
										TEntity entity,
										TimeSpan? expiration = null,
										CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(key);


		if (expiration.HasValue &&
			expiration.Value <= TimeSpan.Zero)
			
			throw new ArgumentOutOfRangeException(nameof(expiration),
												  expiration,
												 "Cache Expiration must be Greater Than Zero.");


		ArgumentNullException.ThrowIfNull(entity);


		string json = JsonSerializer.Serialize(value: entity,
											   options: _jsonOptions);

		bool wasSet = await _database.StringSetAsync(key: key,
													 value: json,
													 expiry: expiration,
													 when: When.Always)
									 .WaitAsync(ct);

		if (!wasSet)
			throw new InvalidOperationException($"Cache value could not be stored for key '{key}'.");
	}


	public async Task<bool> RemoveAsync(string key, CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(key);


		return await _database.KeyDeleteAsync(key)
							  .WaitAsync(ct);
	}


	public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(key);


		return await _database.KeyExistsAsync(key)
							  .WaitAsync(ct);
	}

}
