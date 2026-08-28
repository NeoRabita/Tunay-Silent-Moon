namespace SilentMoonApp.WebAPI.Contracts.Courses.GetCourseWithNarrators;

public sealed class GetCourseWithNarratorsResponse
{
	public Guid Id { get; init; }
	public string Title { get; init; } = null!;
	public string SubTitle { get; init; } = null!;
	public string CategoryType { get; init; } = null!;
	public Guid CategoryId { get; init; }
	public string ImageUrl { get; init; } = string.Empty;
	public int DurationSec { get; init; }
	public bool IsFeatured { get; init; }
	public IReadOnlyList<string> Narrators { get; init; } = [];
	public string Description { get; init; } = null!;
	public int TrackCount { get; init; }
	public DateTimeOffset CreatedAt { get; init; }
}
