using SilentMoonApp.WebAPI.Contracts.Common;


namespace SilentMoonApp.WebAPI.Contracts.Courses.GetCourses;

public sealed class GetCoursesResponse
{
	public PaginationResponse<GetCourseItemResponse> PaginationResponse { get; init; } = null!;
}


public sealed class GetCourseItemResponse
{
	public Guid Id { get; init; }
	public string Title { get; init; } = null!;
	public string SubTitle { get; init; } = null!;
	public string CategoryType { get; init; } = null!;
	public string CategoryName { get; init; } = null!;
	public Guid? CategoryId { get; init; }
	public string? ImageUrl { get; init; }
	public int DurationSec { get; init; }
	public bool IsFeatured { get; init; }
	public IReadOnlyList<string> Narrators { get; init; } = null!;
}