using SilentMoonApp.Application.Abstractions.Repositories.Write;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Infrastructure.Persistence.Contexts;

namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Write;

public class RefreshTokenWriteRepository : WriteRepository<RefreshToken>, IRefreshTokenWriteRepository
{
	public RefreshTokenWriteRepository(AppDbContext dbContext) : base(dbContext) { }
}
