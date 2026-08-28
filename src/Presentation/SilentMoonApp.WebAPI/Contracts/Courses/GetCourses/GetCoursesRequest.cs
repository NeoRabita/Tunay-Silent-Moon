using SilentMoonApp.Domain.Enums;
using SilentMoonApp.WebAPI.Contracts.Common;
using System.Text.Json.Serialization;


namespace SilentMoonApp.WebAPI.Contracts.Courses.GetCourses;

public sealed class GetCoursesRequest
{
	//public PaginationRequest PaginationRequest { get; init; } = null!;

	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = 20;

	public ECourseSortBy CourseSortBy { get; init; } = ECourseSortBy.CreatedAt;
	public ESortDirection SortDirection { get; init; } = ESortDirection.Descending;

	public string? CategoryType { get; init; }
	public Guid? CategoryId { get; init; }
	public string? Search { get; init; }
	public bool? IsFeatured { get; init; }
}
