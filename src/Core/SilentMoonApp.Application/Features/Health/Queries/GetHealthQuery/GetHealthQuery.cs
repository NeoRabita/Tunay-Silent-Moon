using SilentMoonApp.Application.Abstractions.Messaging;


namespace SilentMoonApp.Application.Features.Health.Queries.GetHealthQuery;

public sealed record GetHealthQuery() : IQuery<GetHealthResult>;
