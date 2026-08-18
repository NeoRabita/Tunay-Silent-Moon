using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Domain.Enums;

namespace SilentMoonApp.Application.Abstractions.Repositories.Read;

public interface IUserExternalProviderReadRepository : IReadRepository<UserExternalProvider>
{
	Task<UserExternalProvider?> GetProviderWithUserAsync(EExternalAuthProvider provider,
														string providerUserId,
														CancellationToken cancellationToken = default);
}
