using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Infrastructure.Persistence.Contexts;

namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Read;

public class UserReadRepository : ReadRepository<User>, IUserReadRepository
{
	public UserReadRepository(AppDbContext dbContext) : base(dbContext) { }

	public async Task<User?> GetByEmailAsync(string email,
											 bool tracking = false,
											 CancellationToken ct = default)

		=> await GetAsync(filter: user => user.Email== email,
						  includes: users => users.Include(user => user.UserRoles)
						  						  .ThenInclude(userRole => userRole.Role),
						  tracking: tracking,
						  ct: ct);
}
