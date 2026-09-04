using SilentMoonApp.WebAPI.Contracts.Common;


namespace SilentMoonApp.WebAPI.Contracts.CourseFavorites.GetMyFavorites;
public sealed class GetMyFavoritesResponse
{
	public IReadOnlyList<GetMyFavoriteItemResponse> Data { get; init; } = [];
	public PaginationResponseMeta Meta { get; init; } = null!;
}


public sealed class GetMyFavoriteItemResponse
{
	public Guid Id { get; init; }
	public Guid CourseId { get; init; }
	public GetMyFavoriteCourseResponse Course { get; init; } = null!;
	public DateTimeOffset CreatedAt { get; init; }
}


public sealed class GetMyFavoriteCourseResponse
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
