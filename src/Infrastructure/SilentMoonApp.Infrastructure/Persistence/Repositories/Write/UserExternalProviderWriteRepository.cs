using SilentMoonApp.Application.Abstractions.Repositories.Write;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Infrastructure.Persistence.Contexts;

namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Write;

public class UserExternalProviderWriteRepository : WriteRepository<UserExternalProvider>, IUserExternalProviderWriteRepository
{
	public UserExternalProviderWriteRepository(AppDbContext dbContext) : base(dbContext) { }

}
