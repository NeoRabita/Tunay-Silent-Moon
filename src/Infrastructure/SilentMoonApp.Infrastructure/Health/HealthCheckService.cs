using SilentMoonApp.Application.Abstractions.Health;
using SilentMoonApp.Infrastructure.Persistence.Contexts;

namespace SilentMoonApp.Infrastructure.Health;

public class HealthCheckService : IHealthCheckService
{
	private readonly AppDbContext _dbContext;

	public HealthCheckService(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}


	public async Task<bool> CanConnectDatabaseAsync(CancellationToken ct = default)
	{
		try
		{
			return await _dbContext.Database.CanConnectAsync(ct);
		}
		catch
		{
			return false;
		}
	}
}
