using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourseDetail;

public sealed record GetCourseDetailQuery(Guid Id,
										  Guid? NarratorId) :IQuery<GetCourseDetailResult>;
