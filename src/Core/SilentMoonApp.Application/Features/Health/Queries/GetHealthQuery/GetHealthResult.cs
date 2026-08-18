namespace SilentMoonApp.Application.Features.Health.Queries.GetHealthQuery;

public sealed record GetHealthResult
(
	string Status,
	DateTimeOffset TimeStamp,
	string Version,
	EDatabaseStatus DatabaseStatus
);
