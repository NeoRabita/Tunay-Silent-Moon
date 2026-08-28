namespace SilentMoonApp.WebAPI.Contracts.Courses.GetRelatedCourses;


public sealed class GetRelatedCoursesResponse
{
	public IReadOnlyList<GetRelatedCourseItemResponse> Courses { get; init; } = [];
}


public sealed class GetRelatedCourseItemResponse
{
	public Guid Id { get; init; }
	public string Title { get; init; } = null!;
	public string SubTitle { get; init; } = null!;
	public string Type { get; init; } = null!;
	public Guid CategoryId { get; init; }
	public string ImageUrl { get; init; } = string.Empty;
	public int DurationSec { get; init; }
	public bool IsFeatured { get; init; }
	public IReadOnlyList<string> Narrators { get; init; } = [];
}

