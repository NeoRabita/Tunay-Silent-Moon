using SilentMoonApp.WebAPI.Contracts.Common;

namespace SilentMoonApp.WebAPI.Contracts.SearchCourses.GetSearchCourses;

public sealed class GetSearchCoursesResponse
{
	public required string Search { get; init; } = string.Empty;
	public IReadOnlyList<GetSearchCourseItemResponse> Data { get; init; } = [];
	public PaginationResponseMeta Meta { get; init; } = null!;
}


public sealed class GetSearchCourseItemResponse
{
	public Guid Id { get; init; }
	public string Title { get; init; } = string.Empty;
	public string SubTitle { get; init; } = string.Empty;
	public string Type { get; init; }= string.Empty;
	public Guid? CategoryId { get; init; }
	public string ImageUrl { get; init; } = string.Empty;
	public int DurationSec { get; init; }
	public bool IsFeatured { get; init; }
	public IReadOnlyList<string> Narrators { get; init; } = [];
}