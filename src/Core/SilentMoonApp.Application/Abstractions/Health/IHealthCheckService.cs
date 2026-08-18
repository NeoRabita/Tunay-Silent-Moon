namespace SilentMoonApp.Application.Abstractions.Health;

public interface IHealthCheckService
{
	Task<bool> CanConnectDatabaseAsync(CancellationToken cancellationToken = default);
}
