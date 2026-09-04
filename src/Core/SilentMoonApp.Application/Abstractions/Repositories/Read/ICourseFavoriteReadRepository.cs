using SilentMoonApp.Domain.Entities;

namespace SilentMoonApp.Application.Abstractions.Repositories.Read;

public interface ICourseFavoriteReadRepository
{
	IQueryable<CourseFavorite> QueryMyFavorites(Guid userId,
												Guid? categoryTypeId);
}
