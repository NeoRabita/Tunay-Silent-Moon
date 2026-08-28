using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourseWithNarrators;

public sealed record GetCourseWithNarratorsQuery(Guid Id) : IQuery<GetCourseWithNarratorsResult>;
