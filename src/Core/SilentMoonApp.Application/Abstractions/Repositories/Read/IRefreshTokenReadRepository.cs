using SilentMoonApp.Domain.Entities.Identity;

namespace SilentMoonApp.Application.Abstractions.Repositories.Read;

public interface IRefreshTokenReadRepository
{
	Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, bool tracking = false,
											CancellationToken cancellationToken = default);

	Task<RefreshToken?> GetByTokenHashWithUsersAsync(string tokenHash,
													 bool tracking = false,
													 CancellationToken cancellationToken = default);

	Task<IReadOnlyList<RefreshToken>> GetAllActivesByUserId(Guid userId,
															DateTimeOffset nowUtc,
															bool tracking = false,
															CancellationToken cancellationToken = default);
}
