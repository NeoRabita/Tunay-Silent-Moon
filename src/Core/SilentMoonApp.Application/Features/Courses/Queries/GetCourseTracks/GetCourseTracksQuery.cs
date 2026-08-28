using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Courses.Queries.GetCourseTracks;

public sealed record GetCourseTracksQuery(Guid Id,
										  Guid? NarratorId) : IQuery<GetCourseTracksResult>;
