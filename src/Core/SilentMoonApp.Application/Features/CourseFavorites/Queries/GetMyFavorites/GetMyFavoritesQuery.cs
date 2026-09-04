using SilentMoonApp.Application.Abstractions.Messaging;
using SilentMoonApp.Application.DTOs.Common;

namespace SilentMoonApp.Application.Features.CourseFavorites.Queries.GetMyFavorites;

public sealed record GetMyFavoritesQuery(PaginationQueryRequest PaginationQueryRequest,
										 Guid? CategoryTypeId) : IQuery<GetMyFavoritesResult>;