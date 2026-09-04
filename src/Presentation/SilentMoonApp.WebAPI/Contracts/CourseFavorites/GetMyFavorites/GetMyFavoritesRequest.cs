using SilentMoonApp.Application.DTOs.Common;

namespace SilentMoonApp.WebAPI.Contracts.CourseFavorites.GetMyFavorites;

public sealed class GetMyFavoritesRequest : PaginationQueryRequest
{
	public Guid? CategoryTypeId { get; set; }
}
