using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.Abstractions.Repositories.Read;
using SilentMoonApp.Application.Abstractions.Repositories;
using SilentMoonApp.Application.Abstractions.Repositories.Write;


namespace SilentMoonApp.Application.Services;

public sealed class AuthTokenService : IAuthTokenService
{
	private readonly IJwtService _jwtService;
	private readonly IRequestContext _requestContext;
	private readonly TimeProvider _timeProvider;
	private readonly IUnitOfWork _unitOfWork;

	public AuthTokenService(IJwtService jwtService,
							IRequestContext requestContext,
							TimeProvider timeProvider,
							IUnitOfWork unitOfWork)	
	{
		_jwtService = jwtService;
		_requestContext = requestContext;
		_timeProvider = timeProvider;
		_unitOfWork = unitOfWork;
	}


	public async Task<AuthSessionResult> GenerateSessionAsync(User user,
															  CancellationToken cancellationToken = default)
	{
		GeneratedAccessTokenResult accessTokenResult = _jwtService.GenerateAccessToken(user);

		GeneratedRefreshTokenResult refreshTokenResult = _jwtService.GenerateRefreshToken();


		RefreshToken refreshToken = refreshTokenResult.RefreshToken;
		
		refreshToken.UserId = user.Id;
		
		refreshToken.IpAddress = _requestContext.IpAddress;
		refreshToken.UserAgent = _requestContext.UserAgent;


		await _unitOfWork.WriteRepository<RefreshToken>().AddAsync(entity: refreshToken,
																   cancellationToken: cancellationToken);

		return new AuthSessionResult
		(
			AccessToken: accessTokenResult.AccessToken,
			RefreshToken: refreshTokenResult.RawRefreshToken,
			RefreshTokenId: refreshToken.Id,
			TokenType: "Bearer",
			AccessTokenExpiresIn: accessTokenResult.AccessTokenExpiryMinutes * 60,
			RefreshTokenExpiresAt: refreshToken.ExpiresAt
		);
	}

	public async Task<Result<RefreshToken>> GetActiveRefreshTokenAsync(string? rawRefreshToken,
																	   CancellationToken cancellationToken = default)
	{
		if (string.IsNullOrWhiteSpace(rawRefreshToken))
			return Result<RefreshToken>.Failure(AuthErrors.InvalidRefreshToken());

		
		string tokenHash = _jwtService.HashRefreshToken(rawRefreshToken);


		RefreshToken? refreshToken = await _unitOfWork.Repository<IRefreshTokenReadRepository>().GetByTokenHashWithUsersAsync(tokenHash: tokenHash,
																															  tracking: true,
																															  cancellationToken: cancellationToken);

		if (refreshToken is null)
			return Result<RefreshToken>.Failure(AuthErrors.InvalidRefreshToken());


		DateTimeOffset nowUtc = _timeProvider.GetUtcNow();


		bool isUsed = refreshToken.UsedAt.HasValue;
		bool isRevoked = refreshToken.RevokedAt.HasValue;
		bool isExpired = refreshToken.ExpiresAt <= nowUtc;


		return isUsed || isRevoked || isExpired
			? Result<RefreshToken>.Failure(AuthErrors.InvalidRefreshToken())
			: Result<RefreshToken>.Success(refreshToken);
	}

	public async Task RevokeActiveRefreshTokensAsync(Guid userId,
													ERevocationReason revocationReason,
													DateTimeOffset nowUtc,
													CancellationToken cancellationToken = default)
	{
		IReadOnlyList<RefreshToken> activeRefreshTokens = await _unitOfWork.Repository<IRefreshTokenReadRepository>().GetAllActivesByUserId(userId: userId,
																																			 nowUtc: nowUtc,
																																			 tracking: true,
																																			 cancellationToken: cancellationToken);

		foreach (RefreshToken refreshToken in activeRefreshTokens)
		{
			refreshToken.RevokedAt = nowUtc;
			refreshToken.RevocationReason = revocationReason;
		}
	}
}
