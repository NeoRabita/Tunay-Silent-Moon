using SilentMoonApp.Domain.Entities;

namespace SilentMoonApp.Application.Abstractions.Repositories.Read;

public interface ICategoryReadRepository : IReadRepository<Category>
{
	Task<IReadOnlyList<Category>> GetAllCategoriesWithTypeAsync(string? typeSlug,
															 bool tracking = false,
															 CancellationToken cancellationToken = default);
}
