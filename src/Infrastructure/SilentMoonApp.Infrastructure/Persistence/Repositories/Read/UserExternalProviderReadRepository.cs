using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Domain.Enums;
using SilentMoonApp.Infrastructure.Persistence.Contexts;


namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Read;

public class UserExternalProviderReadRepository : ReadRepository<UserExternalProvider>, IUserExternalProviderReadRepository
{
	private readonly AppDbContext _dbContext;

	public UserExternalProviderReadRepository(AppDbContext dbContext) : base(dbContext)
	{
		_dbContext = dbContext;
	}


	public async Task<UserExternalProvider?> GetProviderWithUserAsync(EExternalAuthProvider provider,
																	  string providerUserId,
																	  CancellationToken ct = default)

		=> await GetAsync(filter: p => p.Provider == provider
									&& p.ProviderUserId == providerUserId,
						  includes: source => source.Include(p => p.User),
						  tracking: false,
						  ct: ct);

}
