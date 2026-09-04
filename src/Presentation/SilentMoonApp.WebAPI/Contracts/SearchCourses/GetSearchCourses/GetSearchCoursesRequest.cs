using SilentMoonApp.Application.DTOs.Common;

namespace SilentMoonApp.WebAPI.Contracts.SearchCourses.GetSearchCourses;

public sealed class GetSearchCoursesRequest : PaginationQueryRequest
{
	public required string Search { get; set; }
	public Guid? CategoryTypeId { get; set; }
}
