using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.DTOs.Common;


namespace SilentMoonApp.Application.Features.TrackProgresses.Queries.GetMyTrackProgressHistory;

public sealed record GetMyTrackProgressHistoryQuery(PaginationQueryRequest PaginationQueryRequest) : IQuery<GetMyTrackProgressHistoryResult>;
