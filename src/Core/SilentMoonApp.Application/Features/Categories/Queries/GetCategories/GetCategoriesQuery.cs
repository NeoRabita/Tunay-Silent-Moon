using SilentMoonApp.Application.Abstractions.Messaging;

namespace SilentMoonApp.Application.Features.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery(string? Type) : IQuery<IReadOnlyList<GetCategoriesResult>>;
