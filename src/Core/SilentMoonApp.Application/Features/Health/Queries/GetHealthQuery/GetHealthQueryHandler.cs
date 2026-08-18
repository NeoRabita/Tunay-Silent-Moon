using SilentMoonApp.Application.Abstractions.Health;
using SilentMoonApp.Application.Abstractions.Messaging;


namespace SilentMoonApp.Application.Features.Health.Queries.GetHealthQuery;

public class GetHealthQueryHandler : IQueryHandler<GetHealthQuery, GetHealthResult>
{
	private readonly IHealthCheckService _healthCheckService;
	private readonly TimeProvider _timeProvider;

	public GetHealthQueryHandler(IHealthCheckService healthCheckService,
								 TimeProvider timeProvider)
	{
		_healthCheckService = healthCheckService;
		_timeProvider = timeProvider;
	}


	public async Task<Result<GetHealthResult>> Handle(GetHealthQuery request, CancellationToken ct = default)
	{
		bool canConnectDatabase = await _healthCheckService.CanConnectDatabaseAsync(ct);

		return Result<GetHealthResult>.Success(
			new GetHealthResult
			(
				Status: "Healthy",
				TimeStamp: _timeProvider.GetUtcNow(),
				Version: typeof(GetHealthQueryHandler).Assembly.GetName().Version?.ToString() ?? "Unknown",
				DatabaseStatus: canConnectDatabase ? EDatabaseStatus.Connected 
												   : EDatabaseStatus.Disconnected
			)
		);
	}
}
