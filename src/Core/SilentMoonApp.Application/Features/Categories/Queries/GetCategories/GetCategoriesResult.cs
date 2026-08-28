namespace SilentMoonApp.Application.Features.Categories.Queries.GetCategories;

public sealed record GetCategoriesResult
(
	Guid Id,
	string Title,
	string Slug,
	string Type,
	string? IconUrl
);