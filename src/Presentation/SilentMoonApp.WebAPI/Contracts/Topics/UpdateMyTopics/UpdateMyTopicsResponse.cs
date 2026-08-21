using SilentMoonApp.WebAPI.Contracts.Topics.GetTopics;


namespace SilentMoonApp.WebAPI.Contracts.Topics.UpdateMyTopics;

public sealed class UpdateMyTopicsResponse
{
	public required IReadOnlyList<UpdateMyTopicResponse> Topics { get; init; } 
}


public sealed class UpdateMyTopicResponse : GetTopicResponse { }