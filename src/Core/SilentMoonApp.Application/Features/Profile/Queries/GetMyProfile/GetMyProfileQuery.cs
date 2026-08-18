using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Profile.Queries.GetMyProfile;

public sealed record GetMyProfileQuery() : IQuery<GetMyProfileResult>;
