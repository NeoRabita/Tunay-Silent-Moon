using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using SilentMoonApp.Application.DTOs.Auth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using SilentMoonApp.Infrastructure.Settings;
using SilentMoonApp.Domain.Entities.Identity;
using SilentMoonApp.Application.Abstractions.Authentication;


namespace SilentMoonApp.Infrastructure.Authentication;

public class JwtService : IJwtService
{
	private static readonly JwtSecurityTokenHandler TokenHandler = new JwtSecurityTokenHandler();

	private readonly JwtSettings _settings;
	private readonly TimeProvider _timeProvider;
	private readonly SigningCredentials _signingCredentials;
	private readonly byte[] _refreshTokenHmacKeyBytes;


	public JwtService(IOptions<JwtSettings> options,
					  TimeProvider timeProvider)
	{
		_settings = options.Value;
		_timeProvider = timeProvider;


		byte[] secretKeyBytes = Encoding.UTF8.GetBytes(_settings.SecretKey);

		_signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKeyBytes), SecurityAlgorithms.HmacSha256);
		
		_refreshTokenHmacKeyBytes = Encoding.UTF8.GetBytes(_settings.RefreshTokenHmacKey);
	}



	public GeneratedAccessTokenResult GenerateAccessToken(User user)
	{
		ArgumentNullException.ThrowIfNull(user, nameof(user));

		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
			new Claim(ClaimTypes.Email, user.Email),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString() ?? string.Empty)
		};

		claims.AddRange(user.UserRoles
							.Select(userRole => userRole.Role.Name.ToString())
							.Distinct(StringComparer.OrdinalIgnoreCase)
							.Select(role => new Claim(ClaimTypes.Role, role)));


		return new GeneratedAccessTokenResult(AccessToken: GenerateJwtToken(claims),
											  ExpiresAt: _timeProvider.GetUtcNow().AddMinutes(_settings.AccessTokenExpirationMinutes),
											  AccessTokenExpiryMinutes: _settings.AccessTokenExpirationMinutes);
	}


	public GeneratedRefreshTokenResult GenerateRefreshToken()
	{
		DateTimeOffset nowUtc = _timeProvider.GetUtcNow();

		string rawRefreshToken = Base64UrlEncoder.Encode(RandomNumberGenerator.GetBytes(64));


		RefreshToken refreshToken = new RefreshToken
		{
			TokenHash = HashRefreshToken(rawRefreshToken),

			CreatedAt = nowUtc,
			ExpiresAt = nowUtc.AddDays(_settings.RefreshTokenExpirationDays),
			UsedAt = null,
			RevokedAt = null,

			RevocationReason = null,

			UserId = Guid.Empty,
			ReplacedTokenId = null,
		};


		return new GeneratedRefreshTokenResult(RawRefreshToken: rawRefreshToken,
											   RefreshToken: refreshToken,
											   RefreshTokenExpiryDays: _settings.RefreshTokenExpirationDays);
	}


	public string HashRefreshToken(string rawRefreshToken)
	{
		ArgumentException.ThrowIfNullOrEmpty(rawRefreshToken, nameof(rawRefreshToken));


		//var secretKeyBytes = Encoding.UTF8.GetBytes(_settings.RefreshTokenHmacKey);

		var refreshTokenbytes = Encoding.UTF8.GetBytes(rawRefreshToken);


		using var hmac = new HMACSHA256(_refreshTokenHmacKeyBytes);

		var hashBytes = hmac.ComputeHash(refreshTokenbytes);


		return Convert.ToBase64String(hashBytes);
	}



	// Helpers

	private string GenerateJwtToken(IEnumerable<Claim> claims)
	{
		DateTimeOffset nowUtc = _timeProvider.GetUtcNow();

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));

		var expires = nowUtc.AddMinutes(_settings.AccessTokenExpirationMinutes).UtcDateTime;

		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


		var token = new JwtSecurityToken(
			issuer: _settings.Issuer,
			audience: _settings.Audience,
			claims: claims,
			notBefore: nowUtc.UtcDateTime,
			expires: expires,
			signingCredentials: _signingCredentials
		);


		return TokenHandler.WriteToken(token);
	}
}
