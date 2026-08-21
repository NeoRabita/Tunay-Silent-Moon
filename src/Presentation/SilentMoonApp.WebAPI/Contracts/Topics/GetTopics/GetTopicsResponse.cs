namespace SilentMoonApp.WebAPI.Contracts.Topics.GetTopics;


public  class GetTopicsResponse
{
	public required IReadOnlyList<GetTopicResponse> Topics { get; init; }
}


public  class GetTopicResponse
{
	public required Guid Id { get; init; }
	public required string Slug { get; init; }
	public required string Title { get; init; }
	public required string IconUrl { get; init; }
	public required string ColorHex { get; init; }
}
