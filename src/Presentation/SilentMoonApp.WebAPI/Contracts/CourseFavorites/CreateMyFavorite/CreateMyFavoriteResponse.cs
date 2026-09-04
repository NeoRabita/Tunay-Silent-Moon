namespace SilentMoonApp.WebAPI.Contracts.CourseFavorites.CreateMyFavorite;

public sealed class CreateMyFavoriteResponse
{
	public Guid Id { get; init; }
	public Guid CourseId { get; init; }
	public CreateMyFavoriteCourseResponse Course { get; init; } = null!;
	public DateTimeOffset CreatedAt { get; init; }
}


public sealed class CreateMyFavoriteCourseResponse
{
	public Guid Id { get; init; }
	public string Title { get; init; } = string.Empty;
	public string SubTitle { get; init; } = string.Empty;
	public Guid CategoryTypeId { get; init; }
	public Guid CategoryId { get; init; }
	public string ImageUrl { get; init; } = string.Empty;
	public int DurationSec { get; init; }
	public bool IsFeatured { get; init; }
	public IReadOnlyList<string> Narrators { get; init; } = [];
}
