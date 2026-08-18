using SilentMoonApp.SharedKernel.Primitives;
using SilentMoonApp.WebAPI.Contracts.Health.GetHealth;
using SilentMoonApp.Application.Abstractions.Messaging.Execution;
using SilentMoonApp.Application.Features.Health.Queries.GetHealthQuery;


namespace SilentMoonApp.WebAPI.Controllers;

[ApiController]
[Route("api/health")]
[Tags("Health")]
public class HealthController : BaseController
{
	private readonly IDispatcher _dispatcher;

	public HealthController(IDispatcher dispatcher)
	{
		_dispatcher = dispatcher;
	}

	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<IActionResult> GetHealthAsync(CancellationToken ct = default)
	{
		GetHealthQuery query = new();

		Result<GetHealthResult> result = await _dispatcher.SendAsync(query: query,
																	  cancellationToken: ct);

		return HandleResult(
			result: result,

			onSuccess: health => Ok(
				new GetHealthResponse
				{
					Status = health.Status,
					TimeStamp = health.TimeStamp,
					Version = health.Version,
					DatabaseStatus = health.DatabaseStatus
				}
			)
		);
	}
}