namespace SilentMoonApp.Application.Features.CourseFavorites.Commands.CreateMyFavorite;


public sealed record CreateMyFavoriteResult(
	Guid Id,
	Guid CourseId,
	CreateMyFavoriteCourseResult Course,
	DateTimeOffset CreatedAt
);


public sealed record CreateMyFavoriteCourseResult(
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