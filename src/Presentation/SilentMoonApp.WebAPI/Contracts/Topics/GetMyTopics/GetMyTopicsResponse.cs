using SilentMoonApp.WebAPI.Contracts.Topics.GetTopics;

namespace SilentMoonApp.WebAPI.Contracts.Topics.GetMyTopics;


public sealed class GetMyTopicsResponse
{
	public required IReadOnlyList<GetMyTopicResponse> Topics { get; init; }
}

public sealed class GetMyTopicResponse : GetTopicResponse { }
