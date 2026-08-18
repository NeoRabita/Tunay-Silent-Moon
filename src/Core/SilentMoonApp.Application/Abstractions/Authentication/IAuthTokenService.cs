using SilentMoonApp.Application.DTOs.Auth;


namespace SilentMoonApp.Application.Abstractions.Authentication;

public interface IAuthTokenService
{
	Task<AuthSessionResult> GenerateSessionAsync(User user,
											     CancellationToken cancellationToken = default);

	Task<Result<RefreshToken>> GetActiveRefreshTokenAsync(string? rawRefreshToken,
														  CancellationToken cancellationToken = default);

	Task RevokeActiveRefreshTokensAsync(Guid userId,
										ERevocationReason revocationReason,
										DateTimeOffset nowUtc,
										CancellationToken cancellationToken = default);
}
