using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourseTrackById;

public sealed record GetCourseTrackByIdQuery(Guid Id) : IQuery<GetCourseTrackByIdResult>;
