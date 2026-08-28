using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Domain.Entities;
using SilentMoonApp.Infrastructure.Persistence.Contexts;


namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Read;

public class CategoryReadRepository : ReadRepository<Category>, ICategoryReadRepository
{
	public CategoryReadRepository(AppDbContext dbContext) : base(dbContext) { }

	public async Task<IReadOnlyList<Category>> GetAllCategoriesWithTypeAsync(string? typeSlug,
																			 bool tracking = false,
																			 CancellationToken ct = default)
		=>await Query(filter: category => !category.IsDeleted
								&& !category.CategoryType.IsDeleted
								&& (typeSlug == null || category.CategoryType.Slug == typeSlug),
				 includes: categories => categories.Include(category => category.CategoryType)
				 								   .Include(category => category.IconFile),
				 tracking: tracking)
			   .ToListAsync(ct);
}
