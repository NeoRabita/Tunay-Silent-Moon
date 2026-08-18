using Microsoft.EntityFrameworkCore;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Infrastructure.Persistence.Contexts;

namespace SilentMoonApp.Infrastructure.Persistence.Repositories.Read;

public class RefreshTokenReadRepository : ReadRepository<RefreshToken>, IRefreshTokenReadRepository
{
	public RefreshTokenReadRepository(AppDbContext dbContext) : base(dbContext) { }

	public Task<RefreshToken?> GetByTokenHashAsync(string tokenHash,
												   bool tracking = false,
												   CancellationToken ct = default)
		=> GetAsync(
			filter: refreshToken => refreshToken.TokenHash == tokenHash,
			tracking: tracking,
			ct: ct);

	public Task<RefreshToken?> GetByTokenHashWithUsersAsync(string tokenHash,
															bool tracking = false,
															CancellationToken ct = default)
		=> GetAsync(
			filter: refreshToken => refreshToken.TokenHash == tokenHash,
			includes: refreshTokens => refreshTokens.Include(refreshToken => refreshToken.User)
													.ThenInclude(user => user.UserRoles)
													.ThenInclude(userRole => userRole.Role),
			tracking: tracking,
			ct: ct);

	public async Task<IReadOnlyList<RefreshToken>> GetAllActivesByUserId(Guid userId,
																		 DateTimeOffset nowUtc,
																		 bool tracking = false,
																		 CancellationToken ct = default)
		=> await Query(
				filter: refreshToken => refreshToken.UserId == userId &&
										refreshToken.ExpiresAt > nowUtc &&
										refreshToken.UsedAt == null &&
										refreshToken.RevokedAt == null,
				tracking: tracking)
			.ToListAsync(ct);
}
