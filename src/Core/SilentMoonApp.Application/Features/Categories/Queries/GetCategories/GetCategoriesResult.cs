namespace SilentMoonApp.Application.Features.Categories.Queries.GetCategories;

public sealed record GetCategoriesResult
(
	Guid Id,
	string Title,
	string Slug,
	string? IconUrl,
	Guid CategoryTypeId,
	string CategoryType,
	string CategoryTypeSlug
);