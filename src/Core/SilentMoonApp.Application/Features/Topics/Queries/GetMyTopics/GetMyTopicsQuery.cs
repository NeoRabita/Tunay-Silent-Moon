using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Topics.Queries.GetMyTopics;

public sealed record GetMyTopicsQuery() : IQuery<IReadOnlyList<GetMyTopicsResult>>;
