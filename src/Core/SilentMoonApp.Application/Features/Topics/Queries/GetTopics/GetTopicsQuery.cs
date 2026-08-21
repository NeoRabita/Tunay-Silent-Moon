using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Topics.Queries.GetTopics;

public sealed record GetTopicsQuery() : IQuery<IReadOnlyList<GetTopicsResult>>;
