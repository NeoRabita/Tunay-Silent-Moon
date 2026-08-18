using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.SharedKernel.Primitives;

namespace SilentMoonApp.Application.Abstractions.Authentication;

public interface IExternalAuthUserService
{
	Task<Result<User>> GetOrGenerateActiveUserAsync(ExternalAuthProviderResult providerResult,
												  CancellationToken cancellationToken = default);
}
