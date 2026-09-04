using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.DTOs.Common;

namespace SilentMoonApp.Application.Features.SearchCourses.Queries.GetSearchCourses;

public sealed record GetSearchCoursesQuery(PaginationQueryRequest PaginationQueryRequest,
										   string Search,
										   Guid? CategoryTypeId) : IQuery<GetSearchCoursesResult>;

