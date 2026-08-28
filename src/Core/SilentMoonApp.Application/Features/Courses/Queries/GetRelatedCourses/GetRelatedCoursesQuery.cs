using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Courses.Queries.GetRelatedCourses;

public sealed record GetRelatedCoursesQuery(Guid Id,
											int Limit) : IQuery<IReadOnlyList<GetRelatedCoursesResult>>;
