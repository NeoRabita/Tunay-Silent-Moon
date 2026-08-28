using SilentMoonApp.Application.DTOs.Common;
using SilentMoonApp.Application.Abstractions.Messaging;


namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourses;

public sealed record GetCoursesQuery(PaginationQueryRequest PaginationRequest,
									 ECourseSortBy? CourseSortBy,
									 ESortDirection? SortDirection,
									 string? CategoryType,
									 Guid? CategoryId,
									 string? Search,
									 bool? IsFeatured) : IQuery<GetCoursesResult>;
