using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using SilentMoonApp.Application.Abstractions.Authentication;
using SilentMoonApp.Application.DTOs.Auth;
using SilentMoonApp.Application.Errors;
using SilentMoonApp.Application.Exceptions.Auth.ExternalAuth;
using SilentMoonApp.Domain.Enums;
using SilentMoonApp.Infrastructure.Settings;
using SilentMoonApp.SharedKernel.Primitives;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;


namespace SilentMoonApp.Infrastructure.Authentication.Providers;

public class FacebookAuthProvider : IExternalAuthProvider
{
	private readonly IConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
	private readonly FacebookAuthSettings _settings;

	public FacebookAuthProvider(IOptions<FacebookAuthSettings> options)
	{
		_settings = options.Value;
		_configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(metadataAddress: _settings.MetaDataAddress,
																					 configRetriever: new OpenIdConnectConfigurationRetriever(),
																					 docRetriever: new HttpDocumentRetriever()
																					 {
																						 RequireHttps = true
																					 });
	}



	public EExternalAuthProvider Provider => EExternalAuthProvider.Facebook;

	public async Task<Result<ExternalAuthProviderResult>> VerifyAsync(string providerToken, CancellationToken cancellationToken = default)
	{
		ArgumentException.ThrowIfNullOrEmpty(providerToken, nameof(providerToken));


		try
		{
			OpenIdConnectConfiguration configuration = await _configurationManager.GetConfigurationAsync(cancellationToken);

			var validationSettings = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKeys = configuration.SigningKeys,

				ValidIssuer = configuration.Issuer,
				ValidAudiences =
				[
					_settings.AppId
				],

				ValidateAudience = true,
				RequireAudience = true,
				ValidAudience = _settings.AppId,

				ValidateLifetime = true,
				RequireExpirationTime = true,

				RequireSignedTokens = true,

				ValidAlgorithms =
				[
					SecurityAlgorithms.RsaSha256
				],

				ClockSkew = TimeSpan.FromMinutes(1)
			};

			var tokenHandler = new JwtSecurityTokenHandler()
			{
				MapInboundClaims = false
			};


			ClaimsPrincipal principal = tokenHandler.ValidateToken(token: providerToken,
																	   validationParameters: validationSettings,
																	   validatedToken: out SecurityToken validatedToken);


			string? providerUserId = principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

			string? email = principal.FindFirst("email")?.Value;

			string? firstName = principal.FindFirst("given_name")?.Value;

			string? lastName = principal.FindFirst("family_name")?.Value;

			string? avatarUrl = principal.FindFirst("picture")?.Value;


			if (string.IsNullOrWhiteSpace(providerUserId))
				return Result<ExternalAuthProviderResult>.Failure(
					ExternalAuthErrors.InvalidProviderToken(externalProvider: EExternalAuthProvider.Facebook));

			if (string.IsNullOrWhiteSpace(email))
				return Result<ExternalAuthProviderResult>.Failure(
					ExternalAuthErrors.EmailRequired());


			return Result<ExternalAuthProviderResult>.Success(
				new ExternalAuthProviderResult
				(
					Provider: EExternalAuthProvider.Facebook,
					ProviderUserId: providerUserId,
					Email: email!,
					FirstName: firstName,
					LastName: lastName,
					AvatarUrl: avatarUrl
				));
		}

		catch (SecurityTokenException)
		{
			return Result<ExternalAuthProviderResult>.Failure(
				ExternalAuthErrors.InvalidProviderToken(externalProvider: EExternalAuthProvider.Facebook));
		}

		catch (HttpRequestException exception)
		{
			throw new ExternalProviderUnavailableException(provider: EExternalAuthProvider.Facebook,
														   innerException: exception);
		}

		catch (JsonException exception)
		{
			throw new ExternalProviderUnavailableException(provider: EExternalAuthProvider.Facebook,
														   innerException: exception);
		}

		catch (OperationCanceledException)
			when (cancellationToken.IsCancellationRequested)
		{
			throw;
		}

		catch (OperationCanceledException exception)
		{
			throw new ExternalProviderUnavailableException(provider: EExternalAuthProvider.Facebook,
														   innerException: exception);
		}

	}





	//try
	//{
	//	FacebookAuthProviderTokenResult debugTokenResult = await ValidateAccessTokenAsync(providerToken, cancellationToken);

	//	FacebookAuthProviderUserResult userResult = await GetUserInfoAsync(providerToken, cancellationToken);


	//	if (!string.Equals(debugTokenResult.UserId, userResult.Id, StringComparison.Ordinal))
	//		throw new InvalidExternalProviderTokenException(EExternalAuthProvider.Facebook);

	//	if (string.IsNullOrEmpty(userResult.Email))
	//		throw new ExternalProviderEmailRequiredException(EExternalAuthProvider.Facebook);


	//	return new ExternalAuthProviderResult
	//	(
	//		Provider: EExternalAuthProvider.Facebook,
	//		ProviderUserId: userResult.Id,
	//		Email: userResult.Email,
	//		FirstName: userResult.FirstName,
	//		LastName: userResult.LastName,
	//		AvatarUrl: userResult.AvatarUrl
	//	);
	//}

	//catch (HttpRequestException exception)
	//{
	//	throw new ExternalProviderUnavailableException(EExternalAuthProvider.Facebook, exception);
	//}

	//catch (JsonException exception)
	//{
	//	throw new ExternalProviderUnavailableException(EExternalAuthProvider.Facebook, exception);
	//}

	//catch (OperationCanceledException exception)
	//	when (exception.CancellationToken == cancellationToken)
	//{
	//	throw new ExternalProviderUnavailableException(EExternalAuthProvider.Facebook, exception);
	//}

	// Helpers 

	//private async Task<FacebookAuthProviderTokenResult> ValidateAccessTokenAsync(string userAccessToken,
	//																		CancellationToken ct = default)
	//{
	//	string requestUrl = $"{_settings.GraphApiVersion}/debug_token?input_token={Uri.EscapeDataString(userAccessToken)}";

	//	string appAccessToken = $"{_settings.AppId}|{_settings.AppSecret}";


	//	using HttpRequestMessage request = new(HttpMethod.Get, requestUrl);

	//	request.Headers.Authorization = new AuthenticationHeaderValue(scheme: "Bearer",
	//																  parameter: appAccessToken);


	//	using HttpResponseMessage response = await _httpClient.SendAsync(request: request,
	//																	 cancellationToken: ct);

	//	if (!response.IsSuccessStatusCode)
	//		throw new InvalidExternalProviderTokenException(EExternalAuthProvider.Facebook);

	//	FacebookAuthProviderTokenResult? debugTokenResult = await response.Content.ReadFromJsonAsync<FacebookAuthProviderTokenResult>(cancellationToken: ct);


	//	if (debugTokenResult is null ||
	//	   !debugTokenResult.IsValid ||
	//		string.IsNullOrWhiteSpace(debugTokenResult.UserId))

	//		throw new InvalidExternalProviderTokenException(EExternalAuthProvider.Facebook);


	//	if (!string.Equals(debugTokenResult.AppId, _settings.AppId, StringComparison.Ordinal))
	//		throw new InvalidExternalProviderTokenException(EExternalAuthProvider.Facebook);

	//	if (debugTokenResult.ExpiresAt > 0)
	//	{
	//		DateTimeOffset expiresAt = DateTimeOffset.FromUnixTimeSeconds(debugTokenResult.ExpiresAt);

	//		if (expiresAt <= _timeProvider.GetUtcNow())
	//			throw new InvalidExternalProviderTokenException(EExternalAuthProvider.Facebook);
	//	}

	//	return debugTokenResult;
	//}


	//private async Task<FacebookAuthProviderUserResult> GetUserInfoAsync(string userAccessToken,
	//																	  CancellationToken ct = default)
	//{
	//	const string fields = "id,email,first_name,last_name,picture.type(large)";

	//	string requestUrl = $"{_settings.GraphApiVersion}/me?fields={Uri.EscapeDataString(fields)}";

	//	using HttpRequestMessage request = new(HttpMethod.Get, requestUrl);

	//	request.Headers.Authorization = new AuthenticationHeaderValue(scheme: "Bearer",
	//																  parameter: userAccessToken);

	//	using HttpResponseMessage response = await _httpClient.SendAsync(request: request,
	//																	 cancellationToken: ct);

	//	if (!response.IsSuccessStatusCode)
	//		throw new InvalidExternalProviderTokenException(EExternalAuthProvider.Facebook);

	//	FacebookAuthProviderUserResult? userResult = await response.Content.ReadFromJsonAsync<FacebookAuthProviderUserResult>(cancellationToken: ct);

	//	if (userResult is null ||
	//		string.IsNullOrWhiteSpace(userResult.Id) ||
	//		string.IsNullOrWhiteSpace(userResult.Email))

	//		throw new InvalidExternalProviderTokenException(EExternalAuthProvider.Facebook);


	//	return userResult;
	//}
}