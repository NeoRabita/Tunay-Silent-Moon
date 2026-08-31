namespace SilentMoonApp.WebAPI.Contracts.Home;


public sealed class GetHomeResponse
{
	public GetHomeGreetingResponse Greeting { get; init; } = null!;

	public GetHomeSectionResponse Recommended { get; init; } = null!;
	public GetHomeSectionResponse? DailyThought { get; init; }
	public GetHomeSectionResponse FeaturedSleep { get; init; } = null!;
	public GetHomeSectionResponse PopularMeditations { get; init; } = null!;
}


public sealed class GetHomeGreetingResponse
{
	public string Title { get; init; } = string.Empty;
	public string Message { get; init; } = string.Empty;
}


public sealed class GetHomeSectionResponse
{
	public string Title { get; init; } = string.Empty;
	public IReadOnlyList<GetHomeCourseItemResponse> Items { get; init; } = [];
}


public sealed class GetHomeCourseItemResponse
{
	public Guid Id { get; init; }
	public string Title { get; init; } = string.Empty;
	public string Subtitle { get; init; } = string.Empty;
	public string Type { get; init; } = string.Empty;
	public Guid CategoryId { get; init; }
	public string ImageUrl { get; init; } = string.Empty;
	public int DurationSec { get; init; }
	public bool IsFeatured { get; init; }
	public IReadOnlyList<string> Narrators { get; init; } = [];
}
