using SilentMoonApp.Application.Abstractions.Repositories.Write;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Infrastructure.Persistence.Contexts;

namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Write;

public class UserWriteRepository : WriteRepository<User>, IUserWriteRepository
{
	public UserWriteRepository(AppDbContext dbContext) : base(dbContext) { }
}
