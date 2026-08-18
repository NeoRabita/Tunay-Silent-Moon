using SilentMoonApp.Application.DTOs.Auth;


namespace SilentMoonApp.Application.Abstractions.Authentication;

public interface IExternalAuthProvider
{
	EExternalAuthProvider Provider { get; }

	Task<Result<ExternalAuthProviderResult>> VerifyAsync(string providerToken,
													   CancellationToken cancellationToken = default);
}
