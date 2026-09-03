using SilentMoonApp.Application.DTOs.Common;
using SilentMoonApp.Application.Features.TrackProgresses.Queries.GetMyTrackProgress;

namespace SilentMoonApp.Application.Features.TrackProgresses.Queries.GetMyTrackProgressHistory;

public sealed record GetMyTrackProgressHistoryResult
(
	PaginationResult<GetMyTrackProgressHistoryItemResult> PaginationResult
);


public sealed record GetMyTrackProgressHistoryItemResult
(
	GetMyTrackProgressResult Progress,
	GetMyTrackProgressHistoryTrackResult Track
);


public sealed record GetMyTrackProgressHistoryTrackResult
(
	Guid Id,
	Guid CourseId,
	string Title,
	string Narrator,
	int DurationSec,
	string AudioUrl,
	string MimeType,
	long FileSizeBytes,
	string ImageUrl,
	int TrackNumber
);