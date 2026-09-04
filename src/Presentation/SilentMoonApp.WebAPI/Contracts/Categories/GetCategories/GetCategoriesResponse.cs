using SilentMoonApp.Domain.Entities;

namespace SilentMoonApp.WebAPI.Contracts.Categories.GetCategories;

public class GetCategoriesResponse
{
	public required IReadOnlyList<GetCategoryResponse> Categories { get; init; } = Array.Empty<GetCategoryResponse>();
}


public class GetCategoryResponse
{
	public required Guid Id { get; init; }
	public required string Title { get; init; }
	public required string Slug { get; init; }
	public string IconUrl { get; init; } = string.Empty;
	public required Guid CategoryTypeId { get; init; }
	public required string CategoryType { get; init; }
	public required string CategoryTypeSlug { get; init; }
}
