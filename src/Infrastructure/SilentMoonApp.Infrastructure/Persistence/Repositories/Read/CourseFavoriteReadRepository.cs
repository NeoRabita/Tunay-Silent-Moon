using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Domain.Entities;
using SilentMoonApp.Infrastructure.Persistence.Contexts;

namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Read;

public class CourseFavoriteReadRepository : ReadRepository<CourseFavorite>, ICourseFavoriteReadRepository
{
	public CourseFavoriteReadRepository(AppDbContext dbContext) : base(dbContext) { }

	public IQueryable<CourseFavorite> QueryMyFavorites(Guid userId, Guid? categoryTypeId)

		=> Query(filter: favorite => favorite.UserId == userId
								  && !favorite.Course.IsDeleted
								  && !favorite.Course.Category.IsDeleted
								  && !favorite.Course.Category.CategoryType.IsDeleted
								  && (categoryTypeId == null || favorite.Course.Category.CategoryTypeId == categoryTypeId),

				 includes: query => query.Include(favorite => favorite.Course)
											.ThenInclude(course => course.Category)
												.ThenInclude(category => category.CategoryType)
										.Include(favorite => favorite.Course)
											.ThenInclude(course => course.CoverImageFile)
										.Include(favorite => favorite.Course)
											.ThenInclude(course => course.Tracks)
												.ThenInclude(track => track.Narrator),
				 tracking: false);
}
