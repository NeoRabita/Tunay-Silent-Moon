namespace SilentMoonApp.WebAPI.Contracts.Topics.UpdateMyTopics;

public sealed class UpdateMyTopicsRequest
{
	public required IReadOnlyList<Guid> TopicIds { get; init; }
}
