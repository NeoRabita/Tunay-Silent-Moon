using SilentMoonApp.Application.DTOs.Common;

namespace SilentMoonApp.Application.Features.CourseFavorites.Queries.GetMyFavorites;


public sealed record GetMyFavoritesResult
(
	PaginationResult<GetMyFavoriteItemResult> PaginationResult
);


public sealed record GetMyFavoriteItemResult
(
	Guid Id,
	Guid CourseId,
	GetMyFavoriteCourseResult Course,
	DateTimeOffset CreatedAt
);


public sealed record GetMyFavoriteCourseResult
(
	Guid Id,
	string Title,
	string SubTitle,
	Guid CategoryTypeId,
	Guid CategoryId,
	string ImageUrl,
	int DurationSec,
	bool IsFeatured,
	IReadOnlyList<string> Narrators
);
